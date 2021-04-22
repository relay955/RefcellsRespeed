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
using RimWorld.Planet;
using UnityEngine;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(Trigger_HighValueThingsAround), "ActivateOn")]
    internal class Trigger_HighvalueThingsAround_ActivateOn
    {
        static bool Prefix(ref Trigger_HighValueThingsAround __instance, ref bool __result, ref Lord lord,ref TriggerSignal signal)
        {
            __result = signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 120 < RefcellRespeedConfig.currentTimeMultiplier && 
                (!TutorSystem.TutorialMode && Find.TickManager.TicksGame - lord.lastPawnHarmTick > 300) &&
                (double) StealAIUtility.TotalMarketValueAround(lord.ownedPawns) > (double) StealAIUtility.StartStealingMarketValueThreshold(lord);
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_KidnapVictimPresent), "ActivateOn")]
    internal class Trigger_KidnapVictimPresent_ActivateOn
    {
        static bool Prefix(ref Trigger_KidnapVictimPresent __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 120 < RefcellRespeedConfig.currentTimeMultiplier)
            {
                if (__instance.data == null || !(__instance.data is TriggerData_PawnCycleInd))
                    BackCompatibility.TriggerDataPawnCycleIndNull(__instance);
                if (Find.TickManager.TicksGame - lord.lastPawnHarmTick > 300)
                {
                    TriggerData_PawnCycleInd data = __instance.Data;
                    ++data.pawnCycleInd;
                    if (data.pawnCycleInd >= lord.ownedPawns.Count)
                        data.pawnCycleInd = 0;
                    if (lord.ownedPawns.Any<Pawn>())
                    {
                        Pawn ownedPawn = lord.ownedPawns[data.pawnCycleInd];
                        if (ownedPawn.Spawned && !ownedPawn.Downed && (ownedPawn.MentalStateDef == null && KidnapAIUtility.TryFindGoodKidnapVictim(ownedPawn, 8f, out Pawn _)) && !GenAI.InDangerousCombat(ownedPawn))
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_WoundedGuestPresent), "ActivateOn")]
    internal class Trigger_WoundedGuestPresent_ActivateOn
    {
        static bool Prefix(ref Trigger_WoundedGuestPresent __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 800 < RefcellRespeedConfig.currentTimeMultiplier)
            {
                TriggerData_PawnCycleInd data = __instance.Data;
                ++data.pawnCycleInd;
                if (data.pawnCycleInd >= lord.ownedPawns.Count)
                    data.pawnCycleInd = 0;
                if (lord.ownedPawns.Any<Pawn>())
                {
                    Pawn ownedPawn = lord.ownedPawns[data.pawnCycleInd];
                    if (ownedPawn.Spawned && !ownedPawn.Downed && (!ownedPawn.InMentalState && KidnapAIUtility.ReachableWoundedGuest(ownedPawn) != null))
                    {
                        __result = true;
                        return false;
                    }
                }
            }
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_ChanceOnTickInteval), "ActivateOn")]
    internal class Trigger_ChanceOnTickInteval_ActivateOn
    {
        static bool Prefix(ref Trigger_ChanceOnTickInteval __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            __result = signal.type == TriggerSignalType.Tick &&
                Find.TickManager.TicksGame % __instance.interval <RefcellRespeedConfig.currentTimeMultiplier &&
                (double) Rand.Value < (double) __instance.chancePerInterval;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_PawnCannotReachMapEdge), "ActivateOn")]
    internal class Trigger_PawnCannotReachMapEdge_ActivateOn
    {
        static bool Prefix(ref Trigger_PawnCannotReachMapEdge __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 197 < RefcellRespeedConfig.currentTimeMultiplier)
            {
                for (int index = 0; index < lord.ownedPawns.Count; ++index)
                {
                    Pawn ownedPawn = lord.ownedPawns[index];
                    if (ownedPawn.Spawned && !ownedPawn.Dead && (!ownedPawn.Downed && !ownedPawn.CanReachMapEdge()))
                    {
                        __result = true;
                        return false;
                    }
                }
            }
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_PawnCanReachMapEdge), "ActivateOn")]
    internal class Trigger_PawnCanReachMapEdge_ActivateOn
    {
        static bool Prefix(ref Trigger_PawnCanReachMapEdge __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            if (signal.type != TriggerSignalType.Tick || Find.TickManager.TicksGame % 193 > RefcellRespeedConfig.currentTimeMultiplier -1)
            {
                __result = false;
                return false;
            }
            for (int index = 0; index < lord.ownedPawns.Count; ++index)
            {
                Pawn ownedPawn = lord.ownedPawns[index];
                if (ownedPawn.Spawned && !ownedPawn.Dead && (!ownedPawn.Downed && !ownedPawn.CanReachMapEdge()))
                {
                __result = false;
                return false;
                }
            }
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_PawnExperiencingDangerousTemperatures), "ActivateOn")]
    internal class  Trigger_PawnExperiencingDangerousTemperatures_ActivateOn
    {
        static bool Prefix(ref Trigger_PawnExperiencingDangerousTemperatures __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 197 < RefcellRespeedConfig.currentTimeMultiplier)
            {
                for (int index = 0; index < lord.ownedPawns.Count; ++index)
                {
                    Pawn ownedPawn = lord.ownedPawns[index];
                    if (ownedPawn.Spawned && !ownedPawn.Dead && !ownedPawn.Downed)
                    {
                        Hediff firstHediffOfDef1 = ownedPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Heatstroke);
                        if (firstHediffOfDef1 != null && (double)firstHediffOfDef1.Severity > (double)__instance.temperatureHediffThreshold)
                        {
                            __result = true;
                            return false;
                        }
                        Hediff firstHediffOfDef2 = ownedPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia);
                        if (firstHediffOfDef2 != null && (double)firstHediffOfDef2.Severity > (double)__instance.temperatureHediffThreshold)
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(Trigger_TickCondition), "ActivateOn")]
    internal class Trigger_TickCondition_ActivateOn
    {
        static bool Prefix(ref Trigger_TickCondition __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
            __result = signal.type == TriggerSignalType.Tick && 
                Find.TickManager.TicksGame % __instance.checkEveryTicks <RefcellRespeedConfig.currentTimeMultiplier &&
                __instance.condition();
            return false;
        }
    }

    [HarmonyPatch]
    internal class Trigger_TicksPassedAfterConditionMet_ActivateOn
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Trigger_TicksPassed), "ActivateOn")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool ActivateOn(Trigger_TicksPassedAfterConditionMet __instance, Lord lord, TriggerSignal signal) { return false; }

        [HarmonyPatch(typeof(Trigger_TicksPassedAfterConditionMet), "ActivateOn")]
        static bool Prefix(ref Trigger_TicksPassedAfterConditionMet __instance, ref bool __result, ref Lord lord, ref TriggerSignal signal)
        {
          if (!__instance.Data.conditionMet && signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % __instance.checkEveryTicks == 0)
            __instance.Data.conditionMet = __instance.condition();
              __result = __instance.Data.conditionMet && ActivateOn(__instance, lord, signal);
            return false;
        }
    }
}
