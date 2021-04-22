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
    internal class MapTemperature_Tick
    {
        [HarmonyPatch(typeof(MapTemperature), "MapTemperatureTick")]
        static bool Prefix(ref MapTemperature __instance)
        {
            float modValue = Find.TickManager.TicksGame % 120;
            if (modValue < 7 && modValue >= 7+RefcellRespeedConfig.currentTimeMultiplier && !DebugSettings.fastEcology)
                return false;
            __instance.fastProcessedRoomGroups.Clear();
            List<Room> allRooms = __instance.map.regionGrid.allRooms;
            for (int index = 0; index < allRooms.Count; ++index)
            {
                RoomGroup group = allRooms[index].Group;
                if (!__instance.fastProcessedRoomGroups.Contains(group))
                {
                    group.TempTracker.EqualizeTemperature();
                    __instance.fastProcessedRoomGroups.Add(group);
                }
            }
            __instance.fastProcessedRoomGroups.Clear();
            return false;
        }
    }
}
