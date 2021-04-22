using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace RefcellRespeed
{
    [StaticConstructorOnStartup]
    internal static class RefcellRespeedStartup
    {
        static RefcellRespeedStartup()
        {
            Harmony harmonyInstance = new Harmony("user.refcells.respeed");
            harmonyInstance.PatchAll();
        }
    }
    class RefcellRespeedConfig : ModSettings
    {
        public static int timeMultiplier = 3;
        public static int currentTimeMultiplier
        {
            get {
                if (adaptiveMode)
                {
                    try
                    {
                        TickManager tickmanager = Find.TickManager;
                        return tickmanager.CurTimeSpeed == TimeSpeed.Paused ? 1 : (int)tickmanager.TickRateMultiplier;
                    }
                    catch
                    {
                        Log.Error("error~");
                        return 1;
                    }
                }
                else
                {
                     return timeMultiplier;
                }
            }
        }

        public static bool adaptiveMode = false;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref timeMultiplier, "timeMultiplier");
            Scribe_Values.Look(ref adaptiveMode, "adaptiveMode");
            base.ExposeData();
        }
    }

    class RefcellRespeedMod : Mod
    {
        RefcellRespeedConfig settings;
        public RefcellRespeedMod(ModContentPack content): base(content)
        {
            this.settings = GetSettings<RefcellRespeedConfig>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ListingStandard = new Listing_Standard();
            ListingStandard.Begin(inRect);
            ListingStandard.Label("Time Multiplier");
            ListingStandard.Label(RefcellRespeedConfig.timeMultiplier.ToString());
            ListingStandard.IntAdjuster(ref RefcellRespeedConfig.timeMultiplier, 1);
            ListingStandard.CheckboxLabeled("Adaptive Mode (for singleplayer only)", ref RefcellRespeedConfig.adaptiveMode,
                " if adaptive mode active, when user change time speeds will be not change tick rate, but calculation will be multiplied. this highly improve animation for user experience, but can only use single player." + 
                "if this mode active, system don't use time multiplier setting.");
            ListingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RefcellRespeed".Translate();
        }
    }
}
