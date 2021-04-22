using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Threading.Tasks;
using UnityEngine;

namespace RefcellRespeed
{
    //public const int TicksPerRealSecond = 20;
    //public const int TickRareInterval = 83;
    //public const int TickLongInterval = 666;

    [HarmonyPatch(typeof(GenTicks),"TicksToSeconds",new Type[]{typeof(int)}) ]
    internal class TicksToSeconds
    {
        static bool Prefix(ref float __result,int numTicks) {__result=(float)numTicks/60f * RefcellRespeedConfig.currentTimeMultiplier; return false; }
    }

    [HarmonyPatch(typeof(GenTicks),"SecondsToTicks",new Type[]{typeof(float)}) ]
    internal class SecondsToTicks
    {
        static bool Prefix(ref int __result, float numSeconds) { __result = Mathf.RoundToInt(60f * numSeconds /RefcellRespeedConfig.currentTimeMultiplier); ; return false; }
    }
}
