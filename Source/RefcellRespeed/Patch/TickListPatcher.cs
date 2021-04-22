using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Threading.Tasks;
using UnityEngine;
using Multiplayer;
using Multiplayer.Client;
using System.Threading;

namespace RefcellRespeed
{
    [HarmonyPatch(typeof(TickList), "Tick")]
    internal class TickListTick
    {
        static bool Prefix(ref TickList __instance)
        {
            for (int index = 0; index < __instance.thingsToRegister.Count; ++index)
                __instance.BucketOf(__instance.thingsToRegister[index]).Add(__instance.thingsToRegister[index]);
            __instance.thingsToRegister.Clear();
            for (int index = 0; index < __instance.thingsToDeregister.Count; ++index)
                __instance.BucketOf(__instance.thingsToDeregister[index]).Remove(__instance.thingsToDeregister[index]);
            __instance.thingsToDeregister.Clear();
            if (DebugSettings.fastEcology)
            {
                Find.World.tileTemperatures.ClearCaches();
                for (int index1 = 0; index1 < __instance.thingLists.Count; ++index1)
                {
                    List<Thing> thingList = __instance.thingLists[index1];
                    for (int index2 = 0; index2 < thingList.Count; ++index2)
                    {
                        if (thingList[index2].def.category == ThingCategory.Plant)
                            thingList[index2].TickLong();
                    }
                }
            }
            int tickValue = Find.TickManager.TicksGame == 0 ? 0 : Find.TickManager.TicksGame/RefcellRespeedConfig.currentTimeMultiplier;
            List<Thing> thingList1 = __instance.thingLists[tickValue % __instance.TickInterval];
            for (int index = 0; index < thingList1.Count; ++index)
            {
                if (!thingList1[index].Destroyed)
                {
                    try
                    {
                        switch (__instance.tickType)
                        {
                            case TickerType.Normal:
                                thingList1[index].Tick();
                                continue;
                            case TickerType.Rare:
                                thingList1[index].TickRare();
                                continue;
                            case TickerType.Long:
                                thingList1[index].TickLong();
                                continue;
                            default:
                                continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        string str = thingList1[index].Spawned ? " (at " + (object)thingList1[index].Position + ")" : "";
                        if (Prefs.DevMode)
                            Log.Error("Exception ticking " + thingList1[index].ToStringSafe<Thing>() + str + ": " + (object)ex);
                        else
                            Log.ErrorOnce("Exception ticking " + thingList1[index].ToStringSafe<Thing>() + str + ". Suppressing further errors. Exception: " + (object)ex, thingList1[index].thingIDNumber ^ 576876901);
                    }
                }
            }
            return false;

        }
    }
}
