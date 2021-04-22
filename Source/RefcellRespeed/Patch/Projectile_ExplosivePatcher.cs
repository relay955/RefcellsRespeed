using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RefcellRespeed
{
    [HarmonyPatch]
    internal class ProjectileExplosive_Impact
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Projectile_Explosive), "Explode")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Explode(Projectile_Explosive instance) { }

        [HarmonyPatch(typeof(Projectile_Explosive), "Impact",new Type[] {typeof(Thing)})]
        static bool Prefix(ref Projectile_Explosive __instance, Thing hitThing, ref bool ___landed, ref int ___ticksToDetonation, ref Projectile ___launcher)
        {
            if (__instance.def.projectile.explosionDelay == 0)
            {
                Explode(__instance);
            }
            else
            {
                ___landed = true;
                ___ticksToDetonation = __instance.def.projectile.explosionDelay / RefcellRespeedConfig.currentTimeMultiplier;
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive((Thing)__instance,
                    __instance.def.projectile.damageDef, ___launcher.Faction);
            }
            return false;
        }
    }
}
