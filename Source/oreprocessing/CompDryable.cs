using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace oreprocessing;

public class CompDryable : ThingComp
{
    private float dryProgressInt;

    public CompProperties_Dryable PropsDry => (CompProperties_Dryable)props;

    public float RotProgressPct => DryProgress / PropsDry.TicksToDry;

    public float DryProgress
    {
        get => dryProgressInt;
        set => dryProgressInt = value;
    }

    public int TicksUntilDryAtCurrentTempHumidity
    {
        get
        {
            var humidity = (Find.WorldGrid[parent.Tile].rainfall / 1000f) + Find.WorldGrid[parent.Tile].swampiness;
            var ambientTemperature = parent.AmbientTemperature;
            ambientTemperature = Mathf.RoundToInt(ambientTemperature);
            return TicksUntilDryAtTempHumidity(ambientTemperature, humidity);
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref dryProgressInt, "dryProg");
    }

    public override void CompTick()
    {
        Tick(1);
    }

    public override void CompTickRare()
    {
        Tick(250);
    }

    private static float DryRateAtTemperature(float temperature)
    {
        if (temperature < 0f)
        {
            return 0f;
        }

        if (temperature >= 10f)
        {
            return temperature / 10f;
        }

        return (temperature - 0f) / 10f;
    }

    private void Tick(int interval)
    {
        if (!ParentIsNotContained(parent.ParentHolder))
        {
            return;
        }

        var num = (Find.WorldGrid[parent.Tile].rainfall / 1000f) + Find.WorldGrid[parent.Tile].swampiness;
        var num2 = DryRateAtTemperature(parent.AmbientTemperature);
        if (ShouldGoWet())
        {
            DryProgress -= 6f * num2 * interval / num;
            if (DryProgress < 0f)
            {
                DryProgress = 0f;
            }
        }
        else
        {
            DryProgress += num2 * interval / num;
        }

        if (DryProgress > PropsDry.TicksToDry)
        {
            TransformIntoSomething();
        }
    }

    public bool ParentIsNotContained(IThingHolder holder)
    {
        if (holder is Map)
        {
            return true;
        }

        return false;
    }

    private void TransformIntoSomething()
    {
        var stackCount = parent.stackCount;
        var forbidden = false;
        var compForbiddable = parent.TryGetComp<CompForbiddable>();
        if (compForbiddable != null)
        {
            forbidden = compForbiddable.Forbidden;
        }

        var map = parent.Map;
        var position = parent.Position;
        parent.DeSpawn();
        parent.Destroy();
        var thing2 = ThingMaker.MakeThing(PropsDry.defDriesTo);
        thing2.stackCount = stackCount;
        GenPlace.TryPlaceThing(thing2, position, map, ThingPlaceMode.Direct, out var lastResultingThing);
        if (forbidden)
        {
            lastResultingThing.SetForbidden(true);
        }
    }

    private bool ShouldGoWet()
    {
        if (parent.ParentHolder is Thing thing && thing.def.category == ThingCategory.Building &&
            thing.def.building.preventDeteriorationInside)
        {
            return false;
        }

        if (parent.Map == null)
        {
            return false;
        }

        if (parent.Map.weatherManager.RainRate > 0f && parent.Map.roofGrid.RoofAt(parent.Position) == null)
        {
            return true;
        }

        var terrainDef = parent.Map.terrainGrid.TerrainAt(parent.Position);
        if (terrainDef.takeSplashes && terrainDef.extraDeteriorationFactor > 0f)
        {
            return true;
        }

        return false;
    }

    public override void PreAbsorbStack(Thing otherStack, int count)
    {
        var t = count / (float)(parent.stackCount + count);
        var dryProgress = ((ThingWithComps)otherStack).GetComp<CompDryable>().DryProgress;
        DryProgress = Mathf.Lerp(DryProgress, dryProgress, t);
    }

    public override void PostSplitOff(Thing piece)
    {
        ((ThingWithComps)piece).GetComp<CompDryable>().DryProgress = DryProgress;
    }

    public override string CompInspectStringExtra()
    {
        var stringBuilder = new StringBuilder();
        if (!(PropsDry.TicksToDry - DryProgress > 0f))
        {
            return stringBuilder.ToString();
        }

        var ticksUntilDryAtCurrentTempHumidity = TicksUntilDryAtCurrentTempHumidity;
        stringBuilder.Append($"TimeToDry:{ticksUntilDryAtCurrentTempHumidity.ToStringTicksToPeriod()}.");

        return stringBuilder.ToString();
    }

    public int TicksUntilDryAtTempHumidity(float temp, float humidity)
    {
        var num = DryRateAtTemperature(temp);
        if (num <= 0f)
        {
            return (int)DryProgress;
        }

        var num2 = PropsDry.TicksToDry - DryProgress;
        if (num2 <= 0f)
        {
            return 0;
        }

        return Mathf.RoundToInt(num2 / num * humidity);
    }
}