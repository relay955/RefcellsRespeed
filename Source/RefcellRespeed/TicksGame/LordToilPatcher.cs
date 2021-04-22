using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RefcellRespeed
{
    [HarmonyPatch]
    internal class LordToil_travel_Tick
    {
        [HarmonyPatch(typeof(LordToil_Travel), "LordToilTick")]
        static bool Prefix(ref LordToil_Travel __instance)
        {
            if (Find.TickManager.TicksGame % 205 > RefcellRespeedConfig.currentTimeMultiplier-1)
                return false;
            LordToilData_Travel data = (LordToilData_Travel)__instance.data;
            bool flag = true;
            for (int index = 0; index < __instance.lord.ownedPawns.Count; ++index)
            {
                Pawn ownedPawn = __instance.lord.ownedPawns[index];
                if (!ownedPawn.Position.InHorDistOf(data.dest, 10) || !ownedPawn.CanReach((LocalTargetInfo)data.dest, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
                return false;
            __instance.lord.ReceiveMemo("TravelArrived");
            return false;
        }
    }

    [HarmonyPatch]
    internal class LordToil_Siege_Tick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LordToil), "LordToilTick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void LordToilTick(LordToil_Siege instance) { }

        [HarmonyPatch(typeof(LordToil_Siege_Tick), "LordToilTick")]
        static bool Prefix(LordToil_Siege __instance)
        {
            LordToilTick(__instance);
            LordToilData_Siege data = __instance.Data;
            if (__instance.lord.ticksInToil == 450)
                __instance.lord.CurLordToil.UpdateAllDuties();
            if (__instance.lord.ticksInToil > 450 && __instance.lord.ticksInToil % 500 == 0)
                __instance.UpdateAllDuties();
            if (Find.TickManager.TicksGame % 500 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            if (!__instance.Frames.Where<Frame>((Func<Frame, bool>)(frame => !frame.Destroyed)).Any<Frame>() && (!data.blueprints.Where<Blueprint>((Func<Blueprint, bool>)(blue => !blue.Destroyed)).Any<Blueprint>() && !__instance.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial).Any<Thing>((Predicate<Thing>)(b => b.Faction == __instance.lord.faction && b.def.building.buildingTags.Contains("Artillery")))))
            {
                __instance.lord.ReceiveMemo("NoArtillery");
            }
            else
            {
                int num1 = GenRadial.NumCellsInRadius(20f);
                int num2 = 0;
                int num3 = 0;
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    IntVec3 c = data.siegeCenter + GenRadial.RadialPattern[index1];
                    if (c.InBounds(__instance.Map))
                    {
                        List<Thing> thingList = c.GetThingList(__instance.Map);
                        for (int index2 = 0; index2 < thingList.Count; ++index2)
                        {
                            if (thingList[index2].def.IsShell)
                                num2 += thingList[index2].stackCount;
                            if (thingList[index2].def == ThingDefOf.MealSurvivalPack)
                                num3 += thingList[index2].stackCount;
                        }
                    }
                }
                if (num2 < 4)
                {
                    ThingDef randomShellDef = TurretGunUtility.TryFindRandomShellDef(ThingDefOf.Turret_Mortar, false, techLevel: __instance.lord.faction.def.techLevel, maxMarketValue: 250f);
                    if (randomShellDef != null)
                        __instance.DropSupplies(randomShellDef, 6);
                }
                if (num3 >= 5)
                    return false;
                __instance.DropSupplies(ThingDefOf.MealSurvivalPack, 12);
            }
            return false;
        }
    }

    [HarmonyPatch]
    internal class LordToil_PrepareCaravan_Leave_Tick
    {
        [HarmonyPatch(typeof(LordToil_PrepareCaravan_Leave), "LordToilTick")]
        static bool Prefix(LordToil_PrepareCaravan_Leave __instance)
        {
            if (Find.TickManager.TicksGame % 100 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
              GatherAnimalsAndSlavesForCaravanUtility.CheckArrived(
                  __instance.lord, __instance.lord.ownedPawns, __instance.exitSpot, "ReadyToExitMap", (Predicate<Pawn>) (x => true));
                return false;
        }
    }

        [HarmonyPatch]
    internal class LordToil_PrepareCaravan_GatherDownedPawn_Tick
    {
        [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherDownedPawns), "LordToilTick")]
        static bool Prefix(LordToil_PrepareCaravan_GatherDownedPawns __instance)
        {
            if (Find.TickManager.TicksGame % 100 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            bool flag = true;
            List<Pawn> downedPawns = ((LordJob_FormAndSendCaravan)__instance.lord.LordJob).downedPawns;
            for (int index = 0; index < downedPawns.Count; ++index)
            {
                if (!JobGiver_PrepareCaravan_GatherDownedPawns.IsDownedPawnNearExitPoint(downedPawns[index], __instance.exitSpot))
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
                return false;
            __instance.lord.ReceiveMemo("AllDownedPawnsGathered");
            return false;
        }
    }


    [HarmonyPatch]
    internal class LordToil_DoOpportunisticTaskOrCover_Tick
    {
        [HarmonyPatch(typeof(LordToil_DoOpportunisticTaskOrCover), "LordToilTick")]
        static bool Prefix(LordToil_DoOpportunisticTaskOrCover __instance)
        {
            if (!__instance.cover || Find.TickManager.TicksGame % 181 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            List<Thing> alreadyTakenTargets = (List<Thing>)null;
            for (int index = 0; index < __instance.lord.ownedPawns.Count; ++index)
            {
                Pawn ownedPawn = __instance.lord.ownedPawns[index];
                if (!ownedPawn.Downed && ownedPawn.mindState.duty.def == DutyDefOf.AssaultColony)
                {
                    Thing target = (Thing)null;
                    if (__instance.TryFindGoodOpportunisticTaskTarget(ownedPawn, out target, alreadyTakenTargets) && !__instance.Map.reservationManager.IsReservedByAnyoneOf((LocalTargetInfo)target, __instance.lord.faction) && !GenAI.InDangerousCombat(ownedPawn))
                    {
                        ownedPawn.mindState.duty = new PawnDuty(__instance.DutyDef);
                        ownedPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        if (alreadyTakenTargets == null)
                            alreadyTakenTargets = new List<Thing>();
                        alreadyTakenTargets.Add(target);
                    }
                }
            }
            return false;
        }
    }

    [HarmonyPatch]
    internal class LordToilGatherItemTickPatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LordToil), "LordToilTick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void LordToilTick(LordToil_PrepareCaravan_GatherItems instance) { }

        [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherItems), "LordToilTick")]
        static bool Prefix(ref LordToil_PrepareCaravan_GatherItems __instance)
        {
            LordToilTick(__instance);
            if (Find.TickManager.TicksGame % 120 < RefcellRespeedConfig.currentTimeMultiplier)
                return false;
            bool flag = true;
            for (int index = 0; index < __instance.lord.ownedPawns.Count; ++index)
            {
                Pawn ownedPawn = __instance.lord.ownedPawns[index];
                if (ownedPawn.IsColonist && ownedPawn.mindState.lastJobTag != JobTag.WaitingForOthersToFinishGatheringItems)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                List<Pawn> allPawnsSpawned = __instance.Map.mapPawns.AllPawnsSpawned;
                for (int index = 0; index < allPawnsSpawned.Count; ++index)
                {
                    if (allPawnsSpawned[index].CurJob != null && allPawnsSpawned[index].jobs.curDriver is JobDriver_PrepareCaravan_GatherItems && allPawnsSpawned[index].CurJob.lord == __instance.lord)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (!flag)
                return false;
            __instance.lord.ReceiveMemo("AllItemsGathered");
            return false;
        }
    } 
}
