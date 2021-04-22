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

namespace RefcellRespeed
{
    
    //스캔 탐색 주기
    [HarmonyPatch]
    internal class FireWatcherTick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(FireWatcher), "UpdateObservations")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void UpdateObservations(FireWatcher instance) {  }

        [HarmonyPatch(typeof(FireWatcher), "FireWatcherTick")]
        static bool Prefix(FireWatcher __instance)
        {
            if (Find.TickManager.TicksGame % 426 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            UpdateObservations(__instance);
            return false;
        }
    }
}
