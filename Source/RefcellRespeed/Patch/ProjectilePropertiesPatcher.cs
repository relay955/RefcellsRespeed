using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(ProjectileProperties), "SpeedTilesPerTick", MethodType.Getter)]
    internal class SpeedTilesPerTick
    {
        static bool Prefix(ref ProjectileProperties __instance, ref float __result)
        {
            __result = __instance.speed / 100f * RefcellRespeedConfig.currentTimeMultiplier;
            return false;
        }
    }
}
