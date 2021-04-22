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
using Verse.AI.Group;

namespace RefcellRespeed
{
    [HarmonyPatch]
    internal class HistoryAutoRecorderPatch
    {
        [HarmonyPatch(typeof(HistoryAutoRecorder), "Tick")]
        static bool Prefix(ref HistoryAutoRecorder __instance)
        {
            if (Find.TickManager.TicksGame % __instance.def.recordTicksFrequency > RefcellRespeedConfig.currentTimeMultiplier - 1 && __instance.records.Any<float>())
                return false;
            __instance.records.Add(__instance.def.Worker.PullRecord());
            return false;
        }
    } 
}
