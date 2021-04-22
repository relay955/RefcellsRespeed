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
    [HarmonyPatch(typeof(Storyteller), "StorytellerTick")]
    internal class StorytellerTick
    {
        static bool Prefix(ref Storyteller __instance)
        {
            __instance.incidentQueue.IncidentQueueTick();
            if (Find.TickManager.TicksGame % 1000 > RefcellRespeedConfig.currentTimeMultiplier-1 || !DebugSettings.enableStoryteller)
               return false;
            foreach (FiringIncident fi in __instance.MakeIncidentsForInterval())
            {
                __instance.TryFire(fi);
            }
            return false;
        }
    }
}
