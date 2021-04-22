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
    [HarmonyPatch(typeof(ResourceCounter), "ResourceCounterTick")]
    internal class ResourceCounterPatcher
    {
        static bool Prefix(ref ResourceCounter __instance)
        {
            if (Find.TickManager.TicksGame % 204 > RefcellRespeedConfig.currentTimeMultiplier-1)
                return false;
            __instance.UpdateResourceCounts();
            return false;
        }
    }
}
