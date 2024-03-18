using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace oreprocessing;

public class OreMapComponent(Map map) : MapComponent(map)
{
    public static readonly List<ThingDef> ResourceDefs =
        DefDatabase<ThingDef>.AllDefs.Where(def => def.deepCommonality > 0f).ToList();

    private List<OreNode> Nodes = [];

    public List<OreNode> GetNodes => Nodes;

    private bool CheckIfSaveHasValidNodes()
    {
        foreach (var node in Nodes)
        {
            if (!node.HasResource)
            {
                return false;
            }
        }

        return true;
    }

    public void RemoveNode(OreNode node)
    {
        Nodes.Remove(node);
    }

    public bool CanScatterAt(IntVec3 pos, Map localMap)
    {
        var ind = CellIndicesUtility.CellToIndex(pos, localMap.Size.x);
        var terrainDef = localMap.terrainGrid.TerrainAt(ind);
        return terrainDef != null &&
               (!terrainDef.IsWater || terrainDef.passability != Traversability.Impassable) &&
               terrainDef.affordances.Contains(OreDefOf.MiningPlatform.terrainAffordanceNeeded);
    }

    public void ScatterResourceAt(IntVec3 c)
    {
        var thingDef = ResourceDefs.RandomElementByWeight(def => def.deepCommonality);
        var numCells = Mathf.CeilToInt(thingDef.deepLumpSizeRange.RandomInRange);
        var list = new List<IntVec3>();
        Log.Message($"ThingDef {thingDef.label}");
        foreach (var item in GridShapeMaker.IrregularLump(c, map, numCells))
        {
            if (!item.InNoBuildEdgeArea(map))
            {
                list.Add(item);
            }
        }

        Nodes.Add(new OreNode(thingDef, list, map));
    }

    public override void MapGenerated()
    {
        var num = OreSettingsHelper.ModSettings.NodesOnMaps - Rand.Range(0, 10);
        while (num > 0)
        {
            if (!CellFinderLoose.TryFindRandomNotEdgeCellWith(10, x => CanScatterAt(x, map), map, out var result))
            {
                continue;
            }

            Log.Message($"Cell to place{result}");
            ScatterResourceAt(result);
            num--;
        }

        Log.Message($"Nodes generated:{Nodes.Count}");
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        if (Nodes.Count <= 0)
        {
            MapGenerated();
            return;
        }

        if (CheckIfSaveHasValidNodes())
        {
            return;
        }

        Log.Message("Map has invalid node, regenerating grid");
        Nodes.Clear();
        MapGenerated();
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref Nodes, "ResourceNodesOnMap", LookMode.Deep);
        base.ExposeData();
    }

    public void MarkForDraw()
    {
        foreach (var node in Nodes)
        {
            node.MarkForDraw();
        }
    }
}