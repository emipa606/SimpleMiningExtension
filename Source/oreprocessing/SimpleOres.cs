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
            VersionFromManifest.GetVersionFromModMetaData(
                ModLister.GetActiveModWithIdentifier("Mlie.SimpleMiningExtension"));
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        if (listing_Standard.ButtonText("SME.Default".Translate()))
        {
            modSettings.Reset();
        }

        listing_Standard.Label("SME.WorkDuration".Translate((int)modSettings.WorkDuration));
        modSettings.WorkDuration = (int)Mathf.Round(listing_Standard.Slider(modSettings.WorkDuration, 1000f, 10000f));
        listing_Standard.Label("SME.MaxOres".Translate(modSettings.NodesOnMaps));
        listing_Standard.IntAdjuster(ref modSettings.NodesOnMaps, 1);

        listing_Standard.Label("SME.Accidents".Translate());
        listing_Standard.CheckboxLabeled("SME.EnableAccidents".Translate(), ref modSettings.AccidentsEnabled);
        listing_Standard.Label("SME.DaysBetweenAccidents".Translate(modSettings.AccidentIntervalDays));
        modSettings.AccidentIntervalDays =
            Mathf.RoundToInt(listing_Standard.Slider(modSettings.AccidentIntervalDays, 1f, 20f));
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("SME.ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    public override string SettingsCategory()
    {
        return "Simple Ores";
    }
}