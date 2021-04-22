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
    //장비 파괴 확인 틱 조정
    [HarmonyPatch(typeof(BreakdownManager), "MapComponentTick")]
    internal class BreakdownManager_MapComponentTick
    {
        static bool Prefix(ref BreakdownManager __instance, ref List<CompBreakdownable> ___comps)
        {
            if (Find.TickManager.TicksGame % BreakdownManager.CheckIntervalTicks > RefcellRespeedConfig.currentTimeMultiplier-1)
                return false;
            for (int index = 0; index < ___comps.Count; ++index)
                ___comps[index].CheckForBreakdown();
            return false;
        }
    }
}
