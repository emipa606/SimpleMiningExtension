using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace oreprocessing;

public class CompProperties_Dryable : CompProperties
{
    public float daysToDry = 2f;

    public ThingDef defDriesTo;

    public CompProperties_Dryable()
    {
        compClass = typeof(CompDryable);
    }

    public CompProperties_Dryable(float daysToDry)
    {
        this.daysToDry = daysToDry;
    }

    public int TicksToDry => Mathf.RoundToInt(daysToDry * 60000f);

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }

        if (parentDef.tickerType != TickerType.Normal && parentDef.tickerType != TickerType.Rare)
        {
            yield return
                $"CompRottable needs tickerType {TickerType.Rare} or {TickerType.Normal}, has {parentDef.tickerType}";
        }
    }
}