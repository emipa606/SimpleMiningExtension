using UnityEngine;
using Verse;

namespace oreprocessing;

public class SimpleOres : Mod
{
    private readonly OreModSettings modSettings;

    public SimpleOres(ModContentPack content)
        : base(content)
    {
        modSettings = GetSettings<OreModSettings>();
        OreSettingsHelper.ModSettings = modSettings;
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        if (listing_Standard.ButtonText("Default Settings"))
        {
            modSettings.Reset();
        }

        listing_Standard.Label(
            $"Work time duration, smaller it is the faster is the mining job {(int)modSettings.WorkDuration}"
                .ToString());
        modSettings.WorkDuration = (int)Mathf.Round(listing_Standard.Slider(modSettings.WorkDuration, 1000f, 10000f));
        listing_Standard.Label($"Number of maximum ore nodes on map: {modSettings.NodesOnMaps}");
        listing_Standard.IntAdjuster(ref modSettings.NodesOnMaps, 1);

        listing_Standard.Label("Mining accidents");
        listing_Standard.CheckboxLabeled("Enable mining accidents?", ref modSettings.AccidentsEnabled);
        listing_Standard.Label($"Minimum days between accidents {modSettings.AccidentIntervalDays}");
        modSettings.AccidentIntervalDays =
            Mathf.RoundToInt(listing_Standard.Slider(modSettings.AccidentIntervalDays, 1f, 20f));
        listing_Standard.End();
    }

    public override string SettingsCategory()
    {
        return "Simple Ores";
    }
}