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
    //GenTicks를 사용한다면 불필요한 패치일 수 있음
    //작업테이블 열 발생 관련 코드로 보임
    [HarmonyPatch(typeof(CompProximityFuse), "CompTick")]
    internal class CompProximityFuse_CompTick
    {
        static bool Prefix(ref CompProximityFuse __instance)
        {
            if (Find.TickManager.TicksGame % 250 > RefcellRespeedConfig.currentTimeMultiplier-1)
                return false;
            __instance.CompTickRare();
            return false;
        }
    }
}
