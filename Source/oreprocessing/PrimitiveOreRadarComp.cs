using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace oreprocessing;

public class PrimitiveOreRadarComp : ThingComp
{
    public static float ResourceChancePer10kCells = DefDatabase<ThingDef>.AllDefs.Where(def => def.deepCommonality > 0f)
        .Sum(def => def.deepCommonality * def.deepLumpSizeRange.Average) / 10000f;

    private readonly ushort ProspectRange = 12;

    private readonly float WorkToCompleteSearch = 10000f;

    public bool canSeeOverlay;

    private float WorkDoneAlready;

    private PrimitiveOreRadarCompProps PropsRadar => (PrimitiveOreRadarCompProps)props;

    private OreMapComponent MapComponent => parent.Map.GetComponent<OreMapComponent>();

    public IEnumerable<IntVec3> GetCellsInMiningRange()
    {
        return GenRadial.RadialCellsAround(parent.Position, ProspectRange, true);
    }

    public IntVec3 ReturnRandomCell()
    {
        var list = GetCellsInMiningRange().ToList().InRandomOrder();
        foreach (var intVec3 in list)
        {
            if (MapComponent.CanScatterAt(intVec3, parent.Map))
            {
                return intVec3;
            }
        }

        return IntVec3.Invalid;
    }

    public IntVec3 ReturnValidCellToDig(Pawn pawn)
    {
        var list = GetCellsInMiningRange().ToList().InRandomOrder();
        foreach (var intVec3 in list)
        {
            if (pawn.CanReserveAndReach(intVec3, PathEndMode.OnCell, Danger.Deadly))
            {
                return intVec3;
            }
        }

        return IntVec3.Invalid;
    }

    public void WorkAtProspectSite(Pawn pawn)
    {
        WorkDoneAlready += 1000f * pawn.GetStatValue(StatDefOf.ResearchSpeed);
        if (!(WorkDoneAlready >= WorkToCompleteSearch))
        {
            return;
        }

        canSeeOverlay = true;
        if (MapComponent.GetNodes.Count >= OreSettingsHelper.ModSettings.NodesOnMaps)
        {
            return;
        }

        var random = new Random();
        var num = 1 - (OreSettingsHelper.ModSettings.NodesOnMaps == 0
            ? 1
            : (float)MapComponent.GetNodes.Count / OreSettingsHelper.ModSettings.NodesOnMaps);
        if ((float)random.NextDouble() <= num)
        {
            MapComponent.ScatterResourceAt(ReturnRandomCell());
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref WorkDoneAlready, "WorkDone");
        Scribe_Values.Look(ref canSeeOverlay, "canSeeOverlay");
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        base.PostDrawExtraSelectionOverlays();
        GenDraw.DrawRadiusRing(parent.Position, ProspectRange);
        if (!canSeeOverlay)
        {
            return;
        }

        foreach (var getNode in MapComponent.GetNodes)
        {
            foreach (var cell in getNode.Cells)
            {
                if (!cell.InHorDistOf(parent.Position, ProspectRange))
                {
                    continue;
                }

                getNode.MarkForDraw();
                break;
            }
        }
    }

    public override string CompInspectStringExtra()
    {
        var text = base.CompInspectStringExtra();
        var f = WorkDoneAlready / WorkToCompleteSearch;
        return !canSeeOverlay ? "SME.Progress".Translate(text, f.ToStringPercent()) : "SME.Finished".Translate(text);
    }
}