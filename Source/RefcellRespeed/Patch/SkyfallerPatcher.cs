using RimWorld;
using Verse;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RefcellRespeed
{
    //드랍포드 속도 조절
    [HarmonyPatch(typeof(Skyfaller), "CurrentSpeed", MethodType.Getter)]
    internal class CurrentSpeed
    {
        static bool Prefix(ref Skyfaller __instance, ref float __result)
        {
            float timeInAnimation = __instance.def.skyfaller.reversed ?   (float)__instance.ticksToImpact / 220f : (float)(1.0 - (double)__instance.ticksToImpact / (double)220f);//ticksToImpactMax
            //애니메이션이 3배 빨라지지만 저멀리서 시작함. 반대로 늦추면 가까이서부터 떨어짐. 오픈시간은 동일
            //tickstoimpact가 120에서 시작했다가 0으로 줄면 도착하고, 120~200으로 설정된것이기본인듯. spawnsetup을 패치해야할거같다.
            __result = __instance.def.skyfaller.speedCurve == null ? __instance.def.skyfaller.speed * RefcellRespeedConfig.currentTimeMultiplier : __instance.def.skyfaller.speedCurve.Evaluate(timeInAnimation) * __instance.def.skyfaller.speed *RefcellRespeedConfig.currentTimeMultiplier;
            return false;
        }
    }
    [HarmonyPatch]
    internal class SpawnSetup
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Thing), "SpawnSetup", new Type[] {typeof(Map),typeof(bool) })]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void BaseSpawnSetup(Skyfaller instance, Map map, bool respawningAfterLoad ) {  }

        [HarmonyPatch(typeof(Skyfaller), "SpawnSetup", new Type[] {typeof(Map),typeof(bool) })]
        static bool Prefix(ref Skyfaller __instance, Map map, bool respawningAfterLoad, ref int ___ticksToImpactMax)
        {
            BaseSpawnSetup(__instance,map, respawningAfterLoad);
            if (respawningAfterLoad)
                return false;
            __instance.ticksToImpact = ___ticksToImpactMax = new IntRange(120,200).RandomInRange / RefcellRespeedConfig.currentTimeMultiplier;
            if (__instance.def.skyfaller.MakesShrapnel)
            {
                float num = GenMath.PositiveMod(__instance.shrapnelDirection, 360f);
                __instance.angle = (double)num >= 270.0 || (double)num < 90.0 ? Rand.Range(-33f, 0.0f) : Rand.Range(0.0f, 33f);
            }
            else
                __instance.angle = __instance.def.skyfaller.angleCurve == null ? -33.7f : __instance.def.skyfaller.angleCurve.Evaluate(0.0f);
            if (!__instance.def.rotatable || !__instance.innerContainer.Any)
                return false;
            __instance.Rotation = __instance.innerContainer[0].Rotation;
            return false;
        }
    }
}
