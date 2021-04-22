using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace RefcellRespeed
{
    //장비 내구도 소모 속도 조정
    [HarmonyPatch]
    internal class GameComponent_OnetimeNotificationPatcher
    {
        [HarmonyPatch(typeof(GameComponent_OnetimeNotification), "GameComponentTick")]
        static bool Prefix(ref GameComponent_OnetimeNotification __instance)
        {
            if (Find.TickManager.TicksGame % 2000 > RefcellRespeedConfig.currentTimeMultiplier-1 || 
                !Rand.Chance(0.05f) || (!__instance.sendAICoreRequestReminder ||
                ResearchProjectTagDefOf.ShipRelated.CompletedProjects() < 2) ||
                (PlayerItemAccessibilityUtility.PlayerOrQuestRewardHas(ThingDefOf.AIPersonaCore) || 
                PlayerItemAccessibilityUtility.PlayerOrQuestRewardHas(ThingDefOf.Ship_ComputerCore)))
                return false;
            Faction relatedFaction = Find.FactionManager.RandomNonHostileFaction();
            if (relatedFaction == null || relatedFaction.leader == null)
                return false;
            Find.LetterStack.ReceiveLetter(
                "LetterLabelAICoreOffer".Translate(),
                "LetterAICoreOffer".Translate((NamedArgument)relatedFaction.leader.LabelDefinite(), 
                (NamedArgument)relatedFaction.Name, relatedFaction.leader.Named("PAWN")).CapitalizeFirst(),
                LetterDefOf.NeutralEvent, (LookTargets)GlobalTargetInfo.Invalid, relatedFaction);
            __instance.sendAICoreRequestReminder = false;
            return false;
        }
    }
}
