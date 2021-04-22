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
    [HarmonyPatch(typeof(SteadyEnvironmentEffects), "SteadyEnvironmentEffectsTick")]
    internal class SteadyEnvironmentEffects_SteadyEnvironmentEffectsTick
    {
        static bool Prefix(ref SteadyEnvironmentEffects __instance)
        {
            if ((double)Find.TickManager.TicksGame % 97.0 < RefcellRespeedConfig.currentTimeMultiplier && Rand.Chance(0.02f))
                __instance.RollForRainFire();
            __instance.outdoorMeltAmount = __instance.MeltAmountAt(__instance.map.mapTemperature.OutdoorTemp);
            __instance.snowRate = __instance.map.weatherManager.SnowRate;
            __instance.rainRate = __instance.map.weatherManager.RainRate;
            __instance.deteriorationRate = Mathf.Lerp(1f, 5f, __instance.rainRate);
            int num = Mathf.CeilToInt((float)__instance.map.Area * 0.0006f);
            int area = __instance.map.Area;
            for (int index = 0; index < num; ++index)
            {
                if (__instance.cycleIndex >= area)
                    __instance.cycleIndex = 0;
                __instance.DoCellSteadyEffects(__instance.map.cellsInRandomOrder.Get(__instance.cycleIndex));
                ++__instance.cycleIndex;
            }
            return false;
        }
    }
}
