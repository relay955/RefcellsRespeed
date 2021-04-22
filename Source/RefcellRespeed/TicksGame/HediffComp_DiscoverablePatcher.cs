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
    internal class HediffComp_Discoverable_Tick
    {
        [HarmonyPatch(typeof(HediffComp_Discoverable), "CompPostTick")]
        static bool Prefix(ref HediffComp_Discoverable __instance)
        {
              if (Find.TickManager.TicksGame % 103 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            __instance.CheckDiscovered();
            return false;
        }
    }
}
