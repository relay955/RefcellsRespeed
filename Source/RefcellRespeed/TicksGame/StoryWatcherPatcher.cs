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
using UnityEngine;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(StoryWatcher_Adaptation), "AdaptationWatcherTick")]
    internal class StoryWatcher_Adaptation_Tick
    {
        static bool Prefix(ref StoryWatcher_Adaptation __instance)
        {
            for (int index = 0; index < __instance.pawnsJustDownedThisTick.Count; ++index)
                __instance.ResolvePawnEvent(__instance.pawnsJustDownedThisTick[index], AdaptationEvent.Downed);
            __instance.pawnsJustDownedThisTick.Clear();
            if (Find.TickManager.TicksGame % 30000 > RefcellRespeedConfig.currentTimeMultiplier-1 ||
                (double)__instance.adaptDays >= 0.0 && (double)GenDate.DaysPassed < (double)__instance.StorytellerDef.adaptDaysGameStartGraceDays)
                return false;
            float num = 0.5f * __instance.StorytellerDef.adaptDaysGrowthRateCurve.Evaluate(__instance.adaptDays);
            if ((double)__instance.adaptDays > 0.0)
                num *= Find.Storyteller.difficultyValues.adaptationGrowthRateFactorOverZero;
            __instance.adaptDays += num;
            __instance.adaptDays = Mathf.Min(__instance.adaptDays, __instance.StorytellerDef.adaptDaysMax);
            return false;
        }
    }

    [HarmonyPatch(typeof(StoryWatcher_PopAdaptation), "PopAdaptationWatcherTick")]
    internal class StoryWatcher_PopAdaptation_Tick
    {
        static bool Prefix(ref StoryWatcher_PopAdaptation __instance)
        {
            //if (Find.TickManager.TicksGame % 30000 != 171) // original
            int modvalue = Find.TickManager.TicksGame % 30000;
            if (modvalue < 171 || modvalue >= 171+RefcellRespeedConfig.currentTimeMultiplier)
                return false;
            __instance.adaptDays += 0.5f;
            return false;
        }
    }
}
