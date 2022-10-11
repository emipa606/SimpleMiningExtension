using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace oreprocessing;

public class OreNode : ICellBoolGiver, IExposable
{
    private BoolGrid boolGrid;

    public List<IntVec3> Cells;

    private CellBoolDrawer drawer;

    private float OreAmount;

    public ThingDef ResourceDef;

    private IntVec3 Size;

    public OreNode(ThingDef thingDef, List<IntVec3> lumpCells, Map map)
    {
        if (thingDef == null)
        {
            Log.Message("Thing def is null");
        }

        if (lumpCells == null || lumpCells.Count == 0)
        {
            Log.Message("Lump cells erro");
        }

        ResourceDef = thingDef;
        Cells = lumpCells;
        Size = map.Size;
        if (lumpCells != null)
        {
            if (thingDef != null)
            {
                OreAmount = lumpCells.Count * (float)thingDef.deepCountPerCell / 10;
            }
        }

        boolGrid = new BoolGrid(map);
        AddToGrid();
    }

    public OreNode()
    {
    }

    public float OreAmountGet => OreAmount;

    private CellBoolDrawer Drawer
    {
        get
        {
            if (drawer == null)
            {
                drawer = new CellBoolDrawer(this, Size.x, Size.z);
            }

            return drawer;
        }
    }

    public bool HasResource => ResourceDef != null;

    public List<IntVec3> GetCells => Cells;

    public Color Color
    {
        get
        {
            try
            {
                if (ResourceDef.stuffProps == null)
                {
                    return Color.red;
                }

                _ = ResourceDef.stuffProps.color;

                return ResourceDef.stuffProps.color;
            }
            catch
            {
                return Color.yellow;
            }
        }
    }

    public bool GetCellBool(int index)
    {
        return boolGrid[index];
    }

    public Color GetCellExtraColor(int index)
    {
        return Color.white;
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref Size, "OreMapSize");
        Scribe_Values.Look(ref OreAmount, "OreOperations");
        Scribe_Collections.Look(ref Cells, "OreCells", LookMode.Value);
        Scribe_Deep.Look(ref boolGrid, "OreBoolGrid");
        Scribe_Defs.Look(ref ResourceDef, "ResourceDef");
    }

    public bool GetCellBool(IntVec3 c)
    {
        return boolGrid[c];
    }

    public void MineChunk(float amount)
    {
        OreAmount -= amount;
    }

    public void RemoveFromGrid()
    {
        foreach (var cell in Cells)
        {
            boolGrid.Set(cell, false);
        }

        Drawer.SetDirty();
    }

    public void MarkForDraw()
    {
        Drawer.CellBoolDrawerUpdate();
        Drawer.MarkForDraw();
        Drawer.CellBoolDrawerUpdate();
    }

    public void AddToGrid()
    {
        if (Cells == null)
        {
            Log.Message("Cells Null");
            return;
        }

        foreach (var cell in Cells)
        {
            boolGrid?.Set(cell, true);
        }

        if (Drawer == null)
        {
            Log.Message("Drawer is null somehow");
            return;
        }

        Drawer.SetDirty();
    }
}