using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace RefcellRespeed
{
    //스킬학습속도 조정
    [HarmonyPatch(typeof(Pawn_SkillTracker), "Learn", new Type[] { typeof(SkillDef),typeof(float),typeof(bool) })]
    internal class Pawn_SkillTracker_Learn
    {
        static bool Prefix(ref Pawn_SkillTracker __instance, ref float xp)
        {
            xp *= RefcellRespeedConfig.currentTimeMultiplier;
            return true;
        }
    }
}
