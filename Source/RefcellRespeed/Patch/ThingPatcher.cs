using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Threading.Tasks;
using UnityEngine;

namespace RefcellRespeed
{
    //작업속도 패치
    [HarmonyPatch(typeof(StatExtension),"GetStatValue",new Type[]{typeof(Thing),typeof(StatDef),typeof(bool)}) ]
    internal class GetStatValue
    {
        static void Postfix(ref float __result,ref StatDef stat) {
            if(stat == StatDefOf.PlantWorkSpeed ||
                stat == StatDefOf.ConstructionSpeed ||
                stat == StatDefOf.GeneralLaborSpeed)
            {
                __result *= RefcellRespeedConfig.currentTimeMultiplier;
            }
        }
    }

}
