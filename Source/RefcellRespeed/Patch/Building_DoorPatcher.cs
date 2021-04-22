using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace RefcellRespeed
{
    //문열리는 속도 조정
    [HarmonyPatch(typeof(Building_Door), "TicksToOpenNow",MethodType.Getter)]
    internal class Building_Door_TicksToOpenNow
    {
        static bool Prefix(ref Building_Door __instance, ref int __result)
        {
            float f = 45f / __instance.GetStatValue(StatDefOf.DoorOpenSpeed) / RefcellRespeedConfig.currentTimeMultiplier;
            if (__instance.DoorPowerOn)
                f *= 0.25f;
            __result = Mathf.RoundToInt(f);
            return false;
        }
    }

    //문 다시 닫히는 시간 조절
    [HarmonyPatch(typeof(Building_Door), "Tick")]
    internal class Building_Door_Tick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Building_Door), "DoorTryClose")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool DoorTryClose(Building_Door instance) { return false; }

        static bool Prefix(ref Building_Door __instance, ref int ___ticksUntilClose)
        {
            if (___ticksUntilClose > 0)
            {
                ___ticksUntilClose -= RefcellRespeedConfig.currentTimeMultiplier -1;
                if (___ticksUntilClose <= 0)___ticksUntilClose = 1;
            }
            return true;
        }
    }
}
