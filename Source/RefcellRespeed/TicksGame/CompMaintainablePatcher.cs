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
    //장비 내구도 소모 속도 조정
    [HarmonyPatch]
    internal class CompMaintainablePatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ThingComp), "CompTick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CompTick(CompMaintainable instance) {}

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CompMaintainable), "CheckTakeDamage")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CheckTakeDamage(CompMaintainable instance) { }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CompMaintainable), "Active",MethodType.Getter)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool Active(CompMaintainable instance) { return false; }

        [HarmonyPatch(typeof(CompMaintainable), "CompTick")]
        static bool Prefix(ref CompMaintainable __instance)
        {
            CompTick(__instance);
            if (!Active(__instance))
                return false;
            ++__instance.ticksSinceMaintain;
            if (Find.TickManager.TicksGame % 250 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            CheckTakeDamage(__instance);
            return false;
        }
    }
}
 
