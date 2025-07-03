using Mlie;
using UnityEngine;
using Verse;

namespace oreprocessing;

public class SimpleOres : Mod
{
    private static string currentVersion;
    private readonly OreModSettings modSettings;

    public SimpleOres(ModContentPack content)
        : base(content)
    {
        modSettings = GetSettings<OreModSettings>();
        OreSettingsHelper.ModSettings = modSettings;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);
        if (listingStandard.ButtonText("SME.Default".Translate()))
        {
            modSettings.Reset();
        }

        listingStandard.Label("SME.WorkDuration".Translate((int)modSettings.WorkDuration));
        modSettings.WorkDuration = (int)Mathf.Round(listingStandard.Slider(modSettings.WorkDuration, 1000f, 10000f));
        listingStandard.Label("SME.MaxOres".Translate(modSettings.NodesOnMaps));
        listingStandard.IntAdjuster(ref modSettings.NodesOnMaps, 1);

        listingStandard.Label("SME.Accidents".Translate());
        listingStandard.CheckboxLabeled("SME.EnableAccidents".Translate(), ref modSettings.AccidentsEnabled);
        listingStandard.Label("SME.DaysBetweenAccidents".Translate(modSettings.AccidentIntervalDays));
        modSettings.AccidentIntervalDays =
            Mathf.RoundToInt(listingStandard.Slider(modSettings.AccidentIntervalDays, 1f, 20f));
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("SME.ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    public override string SettingsCategory()
    {
        return "Simple Ores";
    }
}