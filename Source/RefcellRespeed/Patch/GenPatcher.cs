using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;
using UnityEngine;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(Gen),"IsHashIntervalTick",new Type[]{typeof(Thing),typeof(int)}) ]
    internal class Gen_IsHashIntervalTick_Thing
    {
        static bool Prefix(ref int interval) {
            interval /= RefcellRespeedConfig.currentTimeMultiplier;
            return true;
        }
    }
    [HarmonyPatch(typeof(Gen), "IsHashIntervalTick", new Type[] { typeof(WorldObject), typeof(int) })]
    internal class Gen_IsHashIntervalTick_WorldObject
    {
        static bool Prefix(ref int interval)
        {
            interval /= RefcellRespeedConfig.currentTimeMultiplier;
            return true;
        }
    }
    [HarmonyPatch(typeof(Gen), "IsHashIntervalTick", new Type[] { typeof(Faction), typeof(int) })]
    internal class Gen_IsHashIntervalTick_Faction
    {
        static bool Prefix(ref int interval)
        {
            interval /= RefcellRespeedConfig.currentTimeMultiplier;
            return true;
        }
    }
}
