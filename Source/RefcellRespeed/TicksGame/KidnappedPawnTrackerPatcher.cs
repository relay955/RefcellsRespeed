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
    
    //스캔 탐색 주기
    [HarmonyPatch]
    internal class KidnappedPawnTracker_Tick
    {
        [HarmonyPatch(typeof(KidnappedPawnsTracker), "KidnappedPawnsTrackerTick")]
        static bool Prefix(KidnappedPawnsTracker __instance, ref List<Pawn> ___kidnappedPawns, ref Faction ___faction)
        {
            for (int index = ___kidnappedPawns.Count - 1; index >= 0; --index)
            {
                if (___kidnappedPawns[index].DestroyedOrNull())
                    ___kidnappedPawns.RemoveAt(index);
            }
            if (Find.TickManager.TicksGame % 15051 < RefcellRespeedConfig.currentTimeMultiplier)
                return false;
            for (int index = ___kidnappedPawns.Count - 1; index >= 0; --index)
            {
                if (Rand.MTBEventOccurs(30f, 60000f, 15051f))
                {
                    ___kidnappedPawns[index].SetFaction(___faction, (Pawn)null);
                    ___kidnappedPawns.RemoveAt(index);
                }
            }
            return false;
        }
    }
}
