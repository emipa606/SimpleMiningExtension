using Verse;

namespace oreprocessing;

public class OreModSettings : ModSettings
{
    private const int AccidentIntervalDaysDefault = 5;
    private const int NodesOnMapsDefault = 30;
    private const float WorkDurationDefault = 8000f;

    public int AccidentIntervalDays = AccidentIntervalDaysDefault;
    public bool AccidentsEnabled;
    public int NodesOnMaps = NodesOnMapsDefault;
    public float WorkDuration = WorkDurationDefault;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref NodesOnMaps, "NodesOnMaps", NodesOnMapsDefault);
        Scribe_Values.Look(ref AccidentIntervalDays, "CTD", AccidentIntervalDaysDefault);
        Scribe_Values.Look(ref WorkDuration, "Work", WorkDurationDefault);
        Scribe_Values.Look(ref AccidentsEnabled, "log");
    }

    public void Reset()
    {
        AccidentIntervalDays = AccidentIntervalDaysDefault;
        WorkDuration = WorkDurationDefault;
        NodesOnMaps = NodesOnMapsDefault;
        AccidentsEnabled = false;
    }
}