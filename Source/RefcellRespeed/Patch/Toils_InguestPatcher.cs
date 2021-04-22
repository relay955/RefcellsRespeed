using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using System.Threading.Tasks;
using UnityEngine;

namespace RefcellRespeed
{
    //음식물 먹는속도 수정
    [HarmonyPatch(typeof(Toils_Ingest),"ChewIngestible",new Type[]{typeof(Pawn),typeof(float),typeof(TargetIndex),typeof(TargetIndex)}) ]
    internal class ChewIngestible
    {
        static bool Prefix(ref float durationMultiplier) 
        {
            durationMultiplier /= RefcellRespeedConfig.currentTimeMultiplier;
            return true;
        }
    }
}
