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
    //우호도 감소 관련
    [HarmonyPatch(typeof(SettlementProximityGoodwillUtility), "CheckSettlementProximityGoodwillChange")]
    internal class SettlementProximityGoodwillUtility_CheckSettlementProximityGoodwillChange
    {
        static bool Prefix()
        {
            if (Find.TickManager.TicksGame == 0 || Find.TickManager.TicksGame % 900000 > RefcellRespeedConfig.currentTimeMultiplier-1)
                return false;
            List<Settlement> settlements = Find.WorldObjects.Settlements;
            SettlementProximityGoodwillUtility.tmpGoodwillOffsets.Clear();
            for (int index = 0; index < settlements.Count; ++index)
            {
                Settlement settlement = settlements[index];
                if (settlement.Faction == Faction.OfPlayer)
                    SettlementProximityGoodwillUtility.AppendProximityGoodwillOffsets(settlement.Tile, SettlementProximityGoodwillUtility.tmpGoodwillOffsets, true, false);
            }
            if (!SettlementProximityGoodwillUtility.tmpGoodwillOffsets.Any<Pair<Settlement, int>>())
                return false;
            SettlementProximityGoodwillUtility.SortProximityGoodwillOffsets(SettlementProximityGoodwillUtility.tmpGoodwillOffsets);
            List<Faction> factionsListForReading = Find.FactionManager.AllFactionsListForReading;
            bool flag = false;
            TaggedString text = "LetterFactionBaseProximity".Translate() + "\n\n" + SettlementProximityGoodwillUtility.ProximityGoodwillOffsetsToString(SettlementProximityGoodwillUtility.tmpGoodwillOffsets);
            for (int index1 = 0; index1 < factionsListForReading.Count; ++index1)
            {
                Faction faction = factionsListForReading[index1];
                if (faction != Faction.OfPlayer)
                {
                    int goodwillChange = 0;
                    for (int index2 = 0; index2 < SettlementProximityGoodwillUtility.tmpGoodwillOffsets.Count; ++index2)
                    {
                        if (SettlementProximityGoodwillUtility.tmpGoodwillOffsets[index2].First.Faction == faction)
                            goodwillChange += SettlementProximityGoodwillUtility.tmpGoodwillOffsets[index2].Second;
                    }
                    FactionRelationKind playerRelationKind = faction.PlayerRelationKind;
                    if (faction.TryAffectGoodwillWith(Faction.OfPlayer, goodwillChange, false, false))
                    {
                        flag = true;
                        faction.TryAppendRelationKindChangedInfo(ref text, playerRelationKind, faction.PlayerRelationKind);
                    }
                }

            }
            if (!flag)
                return false;
            Find.LetterStack.ReceiveLetter((string)"LetterLabelFactionBaseProximity".Translate(), (string)text, LetterDefOf.NegativeEvent);
            return false;
        }
    }
}
