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
    //동물 애교 관련 틱 수정
    [HarmonyPatch(typeof(PawnUtility), "GainComfortFromCellIfPossible",new Type[] { typeof(Pawn),typeof(bool)})]
    internal class PawnUtility_GainComfortFromCellIfPossible
    {
        static bool Prefix(ref Pawn p, ref bool chairsOnly)
        {
            if (Find.TickManager.TicksGame % 10 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            Building edifice = p.Position.GetEdifice(p.Map);
            if (edifice == null || chairsOnly && (edifice.def.category != ThingCategory.Building || !edifice.def.building.isSittable))
                return false;
            PawnUtility.GainComfortFromThingIfPossible(p, (Thing)edifice);
            return false;
        }
    }

    [HarmonyPatch(typeof(PawnUtility), "GainComfortFromThingIfPossible", new Type[] { typeof(Pawn), typeof(Thing) })]
    internal class PawnUtility_GainComfortFromCellIfPossible_2
    {
        static bool Prefix(ref Pawn p, ref Thing from)
        {
            if (Find.TickManager.TicksGame % 10 > RefcellRespeedConfig.currentTimeMultiplier -1)
                return false;
            float statValue = from.GetStatValue(StatDefOf.Comfort);
            if ((double)statValue < 0.0 || p.needs == null || p.needs.comfort == null)
                return false;
            p.needs.comfort.ComfortUsed(statValue);
            return false;
        }
    }

}
