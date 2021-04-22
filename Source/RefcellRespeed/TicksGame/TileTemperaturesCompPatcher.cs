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
using RimWorld.Planet;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(TileTemperaturesComp), "WorldComponentTick")]
    internal class TileTemperaturesCompPatcher_WorldComponentTick
    {
        static bool Prefix(ref TileTemperaturesComp __instance)
        {
            for (int index = 0; index < __instance.usedSlots.Count; ++index)
                __instance.cache[__instance.usedSlots[index]].CheckCache();
            int modvalue = Find.TickManager.TicksGame % 300;
            if ((modvalue < 84 &&
                modvalue >= 84+RefcellRespeedConfig.currentTimeMultiplier)|| 
                !__instance.usedSlots.Any<int>())
                return false;
            __instance.cache[__instance.usedSlots[0]] = (TileTemperaturesComp.CachedTileTemperatureData)null;
            __instance.usedSlots.RemoveAt(0);
            return false;
        }
    }
}
