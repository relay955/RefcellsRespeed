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
    [HarmonyPatch(typeof(VoluntarilyJoinableLordsStarter), "Tick_TryStartRandomGathering")]
    internal class VoluntarilyJoinableLordsStarterPatcher
    {
        static bool Prefix(ref VoluntarilyJoinableLordsStarter __instance)
        {
            if (!__instance.map.IsPlayerHome || Find.TickManager.TicksGame % 5000 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            if (Rand.MTBEventOccurs(40f, 60000f, 5000f))
                __instance.startRandomGatheringASAP = true;
            if (!__instance.startRandomGatheringASAP || Find.TickManager.TicksGame - __instance.lastLordStartTick < 600000)
                return false;
            __instance.TryStartRandomGathering();
            return false;
        }
    }
}
