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
using UnityEngine;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(WildAnimalSpawner), "WildAnimalSpawnerTick")]
    internal class WildAnimalSpawnerTick
    {
        static bool Prefix(ref WildAnimalSpawner __instance)
        {
            if (Find.TickManager.TicksGame % 1213 < RefcellRespeedConfig.currentTimeMultiplier ||
                __instance.AnimalEcosystemFull ||
                !Rand.Chance(0.02695556f * __instance.DesiredAnimalDensity))
                return false;
            TraverseParms traverseParms = TraverseParms.For(TraverseMode.NoPassClosedDoors);
            IntVec3 result;
            Map map = __instance.map;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out result, __instance.map, CellFinder.EdgeRoadChance_Animal, true, (Predicate<IntVec3>)
                (cell => map.reachability.CanReachMapEdge(cell, traverseParms))))
                return false;
            __instance.SpawnRandomWildAnimalAt(result);
            return false;
        }
    }
}
