using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;
using System.Runtime.CompilerServices;

namespace RefcellRespeed
{


    [HarmonyPatch(typeof(Pawn), "Tick")]
    internal class PawnTick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ThingWithComps), "Tick")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Tick(Pawn instance) { }

        static bool Prefix(ref Pawn __instance)
        {
            if (DebugSettings.noAnimals && __instance.Spawned && __instance.RaceProps.Animal)
            {
                __instance.Destroy(DestroyMode.Vanish);
            }
            else
            {
                Tick(__instance);
                if (Find.TickManager.TicksGame % 250 < RefcellRespeedConfig.currentTimeMultiplier)
                    __instance.TickRare();
                int num = __instance.Suspended ? 1 : 0;
                if (num == 0)
                {
                    if (__instance.Spawned)
                        __instance.pather.PatherTick();
                    if (__instance.Spawned)
                    {
                        __instance.stances.StanceTrackerTick();
                        __instance.verbTracker.VerbsTick();
                    }
                    if (__instance.Spawned)
                        __instance.natives.NativeVerbsTick();
                    if (__instance.Spawned)
                        __instance.jobs.JobTrackerTick();
                    if (__instance.Spawned)
                    {
                        __instance.Drawer.DrawTrackerTick();
                        __instance.rotationTracker.RotationTrackerTick();
                    }
                    __instance.health.HealthTick();
                    if (!__instance.Dead)
                    {
                        __instance.mindState.MindStateTick();
                        __instance.carryTracker.CarryHandsTick();
                    }
                }
                if (!__instance.Dead)
                    __instance.needs.NeedsTrackerTick();
                if (num != 0)
                    return false;
                if (__instance.equipment != null)
                    __instance.equipment.EquipmentTrackerTick();
                if (__instance.apparel != null)
                    __instance.apparel.ApparelTrackerTick();
                if (__instance.interactions != null && __instance.Spawned)
                    __instance.interactions.InteractionsTrackerTick();
                if (__instance.caller != null)
                    __instance.caller.CallTrackerTick();
                if (__instance.skills != null)
                    __instance.skills.SkillsTick();
                if (__instance.abilities != null)
                    __instance.abilities.AbilitiesTick();
                if (__instance.inventory != null)
                    __instance.inventory.InventoryTrackerTick();
                if (__instance.drafter != null)
                    __instance.drafter.DraftControllerTick();
                if (__instance.relations != null)
                    __instance.relations.RelationsTrackerTick();
                if (ModsConfig.RoyaltyActive && __instance.psychicEntropy != null)
                    __instance.psychicEntropy.PsychicEntropyTrackerTick();
                if (__instance.RaceProps.Humanlike)
                    __instance.guest.GuestTrackerTick();
                if (__instance.royalty != null && ModsConfig.RoyaltyActive)
                    __instance.royalty.RoyaltyTrackerTick();
                __instance.ageTracker.AgeTick();
                __instance.records.RecordsTick();
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Pawn), "TicksPerMove", new Type[] { typeof(bool) })]
    internal class TicksPerMove
    {
        static bool Prefix(ref Pawn __instance,ref int __result , ref bool diagonal)
        {
            float statValue = __instance.GetStatValue(StatDefOf.MoveSpeed);
            if (RestraintsUtility.InRestraints(__instance))
                statValue *= 0.35f;
            if (__instance.carryTracker != null && __instance.carryTracker.CarriedThing != null && __instance.carryTracker.CarriedThing.def.category == ThingCategory.Pawn)
                statValue *= 0.6f;
            //속도 증폭시키면 원본만큼 체감이 상승되지 않음(틱 검사시간때문에 그런것으로 보임), 따라서 배율에 따른 계수 추가
            float num = statValue /  60f * (RefcellRespeedConfig.currentTimeMultiplier + ((RefcellRespeedConfig.currentTimeMultiplier - 1) * 0.2f));
            float f;
            if ((double)num == 0.0)
            {
                f = 450f;
            }
            else
            {
                f = 1f / num;
                if (__instance.Spawned && !__instance.Map.roofGrid.Roofed(__instance.Position))
                    f /= __instance.Map.weatherManager.CurMoveSpeedMultiplier;
                if (diagonal)
                    f *= 1.41421f;
            }
            __result = Mathf.Clamp(Mathf.RoundToInt(f), 1, 450);
            return false;
        }
    }
}
