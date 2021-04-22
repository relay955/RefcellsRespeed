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
    internal class CompTemperatureDamaged_CompTick
    {
        [HarmonyPatch(typeof(CompTemperatureDamaged), "CompTick")]
        static bool Prefix(ref CompTemperatureDamaged __instance)
        {
              if (Find.TickManager.TicksGame % 250 > RefcellRespeedConfig.currentTimeMultiplier - 1)
                return false;
            __instance.CheckTakeDamage();
            return false;
        }
    }
}
 
