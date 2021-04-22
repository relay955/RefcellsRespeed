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
    internal class CompScanner_TickDoesFind
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CompScanner), "Props",MethodType.Getter)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static CompProperties_Scanner Props(CompScanner instance) { return null; }

        [HarmonyPatch(typeof(CompScanner), "TickDoesFind")]
        [HarmonyPatch(new Type[] {typeof(float)})]
        static bool Prefix(ref CompScanner __instance, ref bool __result, ref float scanSpeed,ref float ___daysWorkingSinceLastFinding)
        {
          __result = Find.TickManager.TicksGame % 59 < RefcellRespeedConfig.currentTimeMultiplier && 
                (Rand.MTBEventOccurs(Props(__instance).scanFindMtbDays / scanSpeed, 60000f, 59f) || 
                (double) Props(__instance).scanFindGuaranteedDays > 0.0 &&
                (double) ___daysWorkingSinceLastFinding >= (double) Props(__instance).scanFindGuaranteedDays);
            return false;
        }
    }
}
