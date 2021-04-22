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
using UnityEngine;

namespace RefcellRespeed
{
    //전력 관련 수정, 에너지량이 3배씩 적용되도록 해야함
    [HarmonyPatch(typeof(PowerNet), "PowerNetTick")]
    internal class PowerNet_Tick
    {
        static bool Prefix(ref PowerNet __instance)
        {
            float extra = __instance.CurrentEnergyGainRate() * RefcellRespeedConfig.currentTimeMultiplier;
            float num1 = __instance.CurrentStoredEnergy();
            if ((double)num1 + (double)extra >= -1.0000000116861E-07 && !__instance.Map.gameConditionManager.ElectricityDisabled)
            {
                if ((__instance.batteryComps.Count <= 0 || (double)num1 < 0.100000001490116 ? (double)num1 : (double)(num1 - 5f)) + (double)extra >= 0.0)
                {
                    PowerNet.partsWantingPowerOn.Clear();
                    for (int index = 0; index < __instance.powerComps.Count; ++index)
                    {
                        if (!__instance.powerComps[index].PowerOn && FlickUtility.WantsToBeOn((Thing)__instance.powerComps[index].parent) && !__instance.powerComps[index].parent.IsBrokenDown())
                            PowerNet.partsWantingPowerOn.Add(__instance.powerComps[index]);
                    }
                    if (PowerNet.partsWantingPowerOn.Count > 0)
                    {
                        int num2 = 200 / PowerNet.partsWantingPowerOn.Count;
                        if (num2 < 30)
                            num2 = 30;
                        if (Find.TickManager.TicksGame % num2 < RefcellRespeedConfig.currentTimeMultiplier)
                        {
                            int num3 = Mathf.Max(1, Mathf.RoundToInt((float)PowerNet.partsWantingPowerOn.Count * 0.05f));
                            for (int index = 0; index < num3; ++index)
                            {
                                CompPowerTrader compPowerTrader = PowerNet.partsWantingPowerOn.RandomElement<CompPowerTrader>();
                                if (!compPowerTrader.PowerOn && (double)extra + (double)num1 >= -((double)compPowerTrader.EnergyOutputPerTick + 1.0000000116861E-07))
                                {
                                    compPowerTrader.PowerOn = true;
                                    extra += compPowerTrader.EnergyOutputPerTick;
                                }
                            }
                        }
                    }
                }
                __instance.ChangeStoredEnergy(extra);
            }
            else
            {
                if (Find.TickManager.TicksGame % 20 > RefcellRespeedConfig.currentTimeMultiplier - 1)
                    return false;
                PowerNet.potentialShutdownParts.Clear();
                for (int index = 0; index < __instance.powerComps.Count; ++index)
                {
                    if (__instance.powerComps[index].PowerOn && (double)__instance.powerComps[index].EnergyOutputPerTick < 0.0)
                        PowerNet.potentialShutdownParts.Add(__instance.powerComps[index]);
                }
                if (PowerNet.potentialShutdownParts.Count <= 0)
                    return false;
                int num2 = Mathf.Max(1, Mathf.RoundToInt((float)PowerNet.potentialShutdownParts.Count * 0.05f));
                for (int index = 0; index < num2; ++index)
                    PowerNet.potentialShutdownParts.RandomElement<CompPowerTrader>().PowerOn = false;
            }
            return false;
        }
    }
}
