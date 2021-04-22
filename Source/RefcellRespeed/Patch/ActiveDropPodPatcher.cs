using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Runtime.CompilerServices;

namespace RefcellRespeed
{
    //드랍포드 전개 딜레이 조정
    [HarmonyPatch]
    internal class ActiveDropPod_Tick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ActiveDropPod), "PodOpen")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void PodOpen(ActiveDropPod instance) { }

        [HarmonyPatch(typeof(ActiveDropPod), "Tick")]
        static bool Prefix(ref ActiveDropPod __instance)
        {
            if (__instance.Contents == null)
                return false;
            __instance.Contents.innerContainer.ThingOwnerTick();
            if (!__instance.Spawned)
                return false;
            __instance.age += RefcellRespeedConfig.currentTimeMultiplier;
            if (__instance.age <= __instance.Contents.openDelay)
                return false;
            PodOpen(__instance);
            return false;
        }
    }
}
