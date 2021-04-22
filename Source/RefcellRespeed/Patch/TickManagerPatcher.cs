using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Threading.Tasks;
using UnityEngine;
using Multiplayer;
using Multiplayer.Client;
using System.Threading;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(TickManager), "DoSingleTick")]
    internal class DoSingleTick
    {
        static bool Prefix(ref TickManager __instance,ref int ___ticksGameInt)
        {
            //Log.Warning("ticksgameint : " + ___ticksGameInt+" ticksgame : "+Find.TickManager.TicksGame);
            ___ticksGameInt += RefcellRespeedConfig.currentTimeMultiplier -1;
            return true;
        }
    }

    [HarmonyPatch(typeof(MapAsyncTimeComp), "Tick")]
    internal class MapAsyncTimeComp_Tick
    {
        static bool Prefix(ref MapAsyncTimeComp __instance)
        {
            __instance.mapTicks+= RefcellRespeedConfig.currentTimeMultiplier-1;
            return true;
        }
    }

    [HarmonyPatch(typeof(TickPatch), "ActualRateMultiplier", new Type[] { typeof(ITickable), typeof(TimeSpeed) })]
    internal class MultiplayerPatchActualRateMultiplier
    {
        static bool Prefix(ref float __result, ITickable tickable, TimeSpeed speed)
        {
            if (MultiplayerWorldComp.asyncTime)
            {
                __result = tickable.TickRateMultiplier(speed) / RefcellRespeedConfig.currentTimeMultiplier;
                return false;
            }

            __result = Find.Maps.Select(m => (ITickable)m.AsyncTime()).Concat(Multiplayer.Client.Multiplayer.WorldComp).Select(t => t.TickRateMultiplier(speed)).Min() / RefcellRespeedConfig.currentTimeMultiplier;
            return false;
        }
    }

    [HarmonyPatch(typeof(TickManager),"CurTimePerTick",MethodType.Getter)]
    internal class CurTimePerTick
    {
        static bool Prefix(ref TickManager __instance, ref float __result)
        {
            if (RefcellRespeedConfig.adaptiveMode)
            {
                __result = (double)__instance.TickRateMultiplier == 0.0 ? 0.0f : (float)1/60;
            }
            else
            {
                __result = (double)__instance.TickRateMultiplier == 0.0 ? 0.0f : (float)(1.0 / (60.0 / RefcellRespeedConfig.currentTimeMultiplier * (double)__instance.TickRateMultiplier));
            }
            return false;
        }
    }
}
