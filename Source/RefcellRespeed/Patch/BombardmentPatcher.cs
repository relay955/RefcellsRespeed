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
    //폭격 탄속 조정
    [HarmonyPatch(typeof(Bombardment.BombardmentProjectile), "Tick")]
    internal class BombardmentProjectile_Tick
    {
        static bool Prefix(ref Bombardment.BombardmentProjectile __instance, ref int ___lifeTime)
        {
            ___lifeTime -= RefcellRespeedConfig.currentTimeMultiplier;
            return false;
        }
    }
 
    //폭격 시간계수 조정
    [HarmonyPatch(typeof(Bombardment), "StartStrike")]
    internal class StartStrike
    {
        static bool Prefix(ref Bombardment __instance)
        {
            __instance.bombIntervalTicks /= RefcellRespeedConfig.currentTimeMultiplier;
            __instance.explosionCount *= RefcellRespeedConfig.currentTimeMultiplier;
            return true;
        }
    }

    /*
    [HarmonyPatch(typeof(Bombardment), "Tick")]
    internal class TickTickTickTest
    {
        static bool Prefix(ref Bombardment __instance)
        {
            Log.Warning("ticksgame:" + Find.TickManager.TicksGame
                + " stattick:" + __instance.startTick
                + " duration:" + __instance.duration
                + " ticksPassed:" + __instance.TicksPassed
                + " ticksleft:" + __instance.TicksLeft
                ) ;
            return true;
        }
    }
    */

    /*
    [HarmonyPatch]
    internal class Bombardment_Tick
    {
        private const int StartRandomFireEveryTicks = 20;
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(OrbitalStrike), "Tick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Tick(Bombardment instance) { }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(OrbitalStrike), "TicksLeft",MethodType.Getter)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static int TicksLeft(Bombardment instance) { return 0; }


        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Bombardment), "StartRandomFire")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void StartRandomFire(Bombardment instance) { }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Bombardment), "EffectTick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void EffectTick(Bombardment instance) { }

        [HarmonyPatch(typeof(Bombardment), "Tick")]
        static bool Prefix(ref Bombardment __instance)
        {
            if (__instance.Destroyed)
                return false;

            if (__instance.warmupTicks > 0)
            {
                if (__instance.warmupTicks <= 0)
                    __instance.StartStrike();
            }
            else
            {
                Tick(__instance);
                if (Find.TickManager.TicksGame % StartRandomFireEveryTicks < RefcellRespeedConfig.currentTimeMultiplier && TicksLeft(__instance) > 0)
                //if (Find.TickManager.TicksGame % (StartRandomFireEveryTicks) == 0&& TicksLeft(__instance) > 0)
                    StartRandomFire(__instance);

            }
            EffectTick(__instance);
            return false;
        }
    }
    */
}
