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
    //폭격 시간계수 조정
    [HarmonyPatch]
    internal class GameCondition_ToxicFallout_Tick
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(GameCondition_ToxicFallout), "DoPawnsToxicDamage",new Type[] { typeof(Map)})]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void DoPawnsToxicDamage(GameCondition_ToxicFallout instance, Map map) { }

        [HarmonyPatch(typeof(GameCondition_ToxicFallout), "GameConditionTick")]
        static bool Prefix(ref GameCondition_ToxicFallout __instance, ref List<SkyOverlay> ___overlays)
        {
            List<Map> affectedMaps = __instance.AffectedMaps;
            if (Find.TickManager.TicksGame % 3451 < RefcellRespeedConfig.currentTimeMultiplier)
            {
                for (int index = 0; index < affectedMaps.Count; ++index)
                    DoPawnsToxicDamage(__instance,affectedMaps[index]);
            }
            for (int index1 = 0; index1 < ___overlays.Count; ++index1)
            {
                for (int index2 = 0; index2 < affectedMaps.Count; ++index2)
                    ___overlays[index1].TickOverlay(affectedMaps[index2]);
            }
            return false;
        }
    }
}
