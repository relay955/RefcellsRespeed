using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RefcellRespeed
{
    //레시피 작업속도 조절
    [HarmonyPatch(typeof(Toils_Recipe), "DoRecipeWork")]
    internal class DoRecipeWork
    {
        static bool Prefix(ref Toil __result)
        {
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBill curDriver = (JobDriver_DoBill)actor.jobs.curDriver;
                UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                if (unfinishedThing != null && unfinishedThing.Initialized)
                {
                    curDriver.workLeft = unfinishedThing.workLeft;
                }
                else
                {
                    curDriver.workLeft = curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff);
                    if (unfinishedThing != null)
                        unfinishedThing.workLeft = curDriver.workLeft;
                }
                curDriver.billStartTick = Find.TickManager.TicksGame;
                curDriver.ticksSpentDoingRecipeWork = 0;
                curJob.bill.Notify_DoBillStarted(actor);
            });
            toil.tickAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBill curDriver = (JobDriver_DoBill)actor.jobs.curDriver;
                UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                if (unfinishedThing != null && unfinishedThing.Destroyed)
                {
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else
                {
                    ++curDriver.ticksSpentDoingRecipeWork;
                    curJob.bill.Notify_PawnDidWork(actor);
                    if (toil.actor.CurJob.GetTarget(TargetIndex.A).Thing is IBillGiverWithTickAction thing2)
                        thing2.UsedThisTick();
                    if (curJob.RecipeDef.workSkill != null && curJob.RecipeDef.UsesUnfinishedThing)
                        actor.skills.Learn(curJob.RecipeDef.workSkill, 0.1f * curJob.RecipeDef.workSkillLearnFactor);
                    float num1 = curJob.RecipeDef.workSpeedStat == null ? 1f : actor.GetStatValue(curJob.RecipeDef.workSpeedStat);
                    num1 *= RefcellRespeedConfig.currentTimeMultiplier;//작업속도 조절 부분
                    Building_WorkTable billGiver = curDriver.BillGiver as Building_WorkTable;
                    if (curJob.RecipeDef.workTableSpeedStat != null)
                        num1 *= billGiver.GetStatValue(curJob.RecipeDef.workTableSpeedStat);
                    if (DebugSettings.fastCrafting)
                        num1 *= 30f;
                    curDriver.workLeft -= num1;
                    if (unfinishedThing != null)
                        unfinishedThing.workLeft = curDriver.workLeft;
                    actor.GainComfortFromCellIfPossible(true);
                    if ((double)curDriver.workLeft <= 0.0)
                    {
                        curDriver.ReadyForNextToil();
                    }
                    else
                    {
                        if (!curJob.bill.recipe.UsesUnfinishedThing)
                            return;
                        int num2 = Find.TickManager.TicksGame - curDriver.billStartTick;
                        if (num2 < 3000 || num2 % 1000 != 0)
                            return;
                        actor.jobs.CheckForJobOverride();
                    }
                }
            });
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect((Func<EffecterDef>)(() => toil.actor.CurJob.bill.recipe.effectWorking), TargetIndex.A);
            toil.PlaySustainerOrSound((Func<SoundDef>)(() => toil.actor.CurJob.bill.recipe.soundWorking));
            toil.WithProgressBar(TargetIndex.A, (Func<float>)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                UnfinishedThing thing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                return (float)(1.0 - (double)((JobDriver_DoBill)actor.jobs.curDriver).workLeft / (double)curJob.bill.recipe.WorkAmountTotal(thing?.Stuff));
            }));
            toil.FailOn<Toil>((Func<bool>)(() =>
            {
                RecipeDef recipeDef = toil.actor.CurJob.RecipeDef;
                if (recipeDef != null && recipeDef.interruptIfIngredientIsRotting)
                {
                    LocalTargetInfo target = toil.actor.CurJob.GetTarget(TargetIndex.B);
                    if (target.HasThing && target.Thing.GetRotStage() > RotStage.Fresh)
                        return true;
                }
                return toil.actor.CurJob.bill.suspended;
            }));
            toil.activeSkill = (Func<SkillDef>)(() => toil.actor.CurJob.bill.recipe.workSkill);
            __result = toil;
            return false;
        }
    }
}
