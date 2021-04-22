using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RefcellRespeed
{
    //연사무기 탄환발사간격 속도 조정
    [HarmonyPatch]
    internal class VerbTick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Verb), "TryCastNextBurstShot")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void TryCastNextBurstShot(Verb instance) { }

       [HarmonyPatch(typeof(Verb), "VerbTick")]
        static bool Prefix(ref Verb __instance, ref int ___ticksToNextBurstShot)
        {
            if (__instance.state != VerbState.Bursting)
                return false;
            if (!__instance.caster.Spawned)
            {
                __instance.Reset();
            }
            else
            {
                ___ticksToNextBurstShot -= RefcellRespeedConfig.currentTimeMultiplier;
                if (___ticksToNextBurstShot > 0)
                    return false;
                TryCastNextBurstShot(__instance);
            }
            return false;
        }
    }
}
