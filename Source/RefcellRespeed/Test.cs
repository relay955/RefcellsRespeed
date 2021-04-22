using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RefcellRespeed
{

        /*
    [HarmonyPatch]
    internal class JobGiver_Wanders2
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(JobGiver_Wander), "GetExactWanderDest",new Type[] {typeof(Pawn)})]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static IntVec3 GetExactWanderDest(JobGiver_Wander instance,Pawn pawn) { return new IntVec3(); }

        [HarmonyPatch(typeof(JobGiver_Wander),"TryGiveJob", new Type[] { typeof(Pawn) })]
        static bool Prefix(ref JobGiver_Wander __instance,ref Job __result, ref Pawn pawn,
            ref IntRange ___ticksBetweenWandersRange, ref int ___expiryInterval, ref LocomotionUrgency ___locomotionUrgency)
        {
            bool flag = pawn.CurJob != null && pawn.CurJob.def == JobDefOf.GotoWander;
            int num = pawn.mindState.nextMoveOrderIsWait ? 1 : 0;
            if (!flag)
                pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
            if (num != 0 && !flag)
            {
                Job job = JobMaker.MakeJob(JobDefOf.Wait_Wander);
                job.expiryInterval = ___ticksBetweenWandersRange.RandomInRange;
                Log.Warning("result : job. " + job.ToString()+" "+job.expiryInterval);
                __result = job;
                return false;
            }
            IntVec3 exactWanderDest = GetExactWanderDest(__instance,pawn);
            if (!exactWanderDest.IsValid)
            {
                pawn.mindState.nextMoveOrderIsWait = false;
                Log.Warning("result : null ");
                __result = null;
                return false;
            }
            Job job1 = JobMaker.MakeJob(JobDefOf.GotoWander, (LocalTargetInfo)exactWanderDest);
            job1.locomotionUrgency = ___locomotionUrgency;
            job1.expiryInterval = ___expiryInterval;
            job1.checkOverrideOnExpire = true;
            Log.Warning("result : job1 "+job1.ToString()+" " +job1.expiryInterval);
            __result = job1;
            return false;
        }

    }
    [HarmonyPatch]
    internal class JobTrackerTick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Pawn_JobTracker), "DetermineNextConstantThinkTreeJob")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static ThinkResult DetermineNextConstantThinkTreeJob(Pawn_JobTracker instance) { return  new ThinkResult(); }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Pawn_JobTracker), "ShouldStartJobFromThinkTree", new Type[] { typeof(ThinkResult)})]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool ShouldStartJobFromThinkTree(Pawn_JobTracker instance,ThinkResult thinkResult) { return  false; }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Pawn_JobTracker), "CheckLeaveJoinableLordBecauseJobIssued", new Type[] { typeof(ThinkResult) })]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CheckLeaveJoinableLordBecauseJobIssued(Pawn_JobTracker instance,ThinkResult thinkResult) { }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Pawn_JobTracker), "FinalizeTick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void FinalizeTick(Pawn_JobTracker instance) { }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Pawn_JobTracker), "CanDoAnyJob")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool CanDoAnyJob(Pawn_JobTracker instance) { return false; }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Pawn_JobTracker), "TryFindAndStartJob")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void TryFindAndStartJob(Pawn_JobTracker instance) { }


        [HarmonyPatch(typeof(Pawn_JobTracker), "JobTrackerTick")]
        static bool Prefix(ref Pawn_JobTracker __instance, ref Pawn ___pawn
            , ref int ___jobsGivenThisTick, ref string ___jobsGivenThisTickTextual)
        {
            ___jobsGivenThisTick = 0;
            ___jobsGivenThisTickTextual = "";
            if (___pawn.IsHashIntervalTick(30))
            {
                ThinkResult constantThinkTreeJob = DetermineNextConstantThinkTreeJob(__instance);
                if (constantThinkTreeJob.IsValid)
                {
                    if (ShouldStartJobFromThinkTree(__instance,constantThinkTreeJob))
                    {
                        CheckLeaveJoinableLordBecauseJobIssued(__instance,constantThinkTreeJob);
                        __instance.StartJob(constantThinkTreeJob.Job, JobCondition.InterruptForced, constantThinkTreeJob.SourceNode, cancelBusyStances: false, thinkTree: ___pawn.thinker.ConstantThinkTree, tag: constantThinkTreeJob.Tag);
                    }
                    else if (constantThinkTreeJob.Job != __instance.curJob && !__instance.jobQueue.Contains(constantThinkTreeJob.Job))
                        JobMaker.ReturnToPool(constantThinkTreeJob.Job);
                }
            }
            if (__instance.curDriver != null)
            {
                if (___pawn.Name != null && ___pawn.Name.ToString().Contains("Alex"))
                {
                    Log.Warning("pawn : " + ___pawn.Name + " job:" + __instance.curJob.def.defName + " expiryinterval : " + __instance.curJob.expiryInterval + "curjob starttick: " + __instance.curJob.startTick+"tick : "+Find.TickManager.TicksGame);
                }
                if (__instance.curJob.expiryInterval > 0 && (Find.TickManager.TicksGame - __instance.curJob.startTick) % __instance.curJob.expiryInterval == 0 && Find.TickManager.TicksGame != __instance.curJob.startTick)
                {
                    if (__instance.curJob.expireRequiresEnemiesNearby && !PawnUtility.EnemiesAreNearby(___pawn, 25))
                    {
                        if (__instance.debugLog)
                            __instance.DebugLogEvent("Job expire skipped because there are no enemies nearby");
                    }
                    else
                    {
                        if (__instance.debugLog)
                            __instance.DebugLogEvent("Job expire");
                        if (!__instance.curJob.checkOverrideOnExpire)
                            __instance.EndCurrentJob(JobCondition.Succeeded);
                        else
                            __instance.CheckForJobOverride();
                        FinalizeTick(__instance);
                        return false;
                    }
                }
                __instance.curDriver.DriverTick();
            }
            if (__instance.curJob == null && !___pawn.Dead && (___pawn.mindState.Active && CanDoAnyJob(__instance)))
            {
                if (__instance.debugLog)
                    __instance.DebugLogEvent("Starting job from Tick because curJob == null.");
                TryFindAndStartJob(__instance);
            }
            FinalizeTick(__instance);
            return false;
        }
    }
    /*
    [HarmonyPatch(typeof(JobDriver), "DriverTick")]
    internal class JobDriverPatch4
    {
        static bool Prefix(ref JobDriver __instance)
        {
            __instance.ticksLeftThisToil -= RefcellRespeedConfig.currentTimeMultiplier - 1;
            return true;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(JobDriver_Wait), "MakeNewToils")]
    internal class JobDriverPatch3
    {
        static bool Prefix(ref JobDriver_Goto __instance)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JobDriver_Goto), "MakeNewToils")]
    internal class JobDriverPatch2
    {
        static bool Prefix(ref JobDriver_Goto __instance)
        {
            try
            {
                throw new Exception();
            }catch(Exception e)
            {
                Log.Error(e.StackTrace);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JobDriver_Goto), "TryExitMap")]
    internal class JobDriverPatch
    {
        static bool Prefix(ref JobDriver_Goto __instance)
        {
            Log.Error("goto tryexitmap called");
            Log.Error("failifcantjoinorcreatecaravan : " + __instance.job.failIfCantJoinOrCreateCaravan);
            Log.Error("canexitmapandjoin : " + !CaravanExitMapUtility.CanExitMapAndJoinOrCreateCaravanNow(__instance.pawn));
            return true;
        }
    }
    */
}
