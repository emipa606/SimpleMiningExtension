using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace oreprocessing;

public class CompMineShaft : ThingComp
{
    protected static readonly FloatRange DamageRandomFactorRange = new FloatRange(0.3f, 1.7f);

    protected static readonly RangeInt DamageCountRange = new RangeInt(1, 4);

    private readonly int AverageDamage = 40;

    private readonly float baseWorkPerCycle = 1500f;

    private readonly int MiningRadius = 8;

    private readonly int ticksInDay = 60000;

    private readonly float WorkNeededForNextPortion = 2000f;

    private bool FirstSpawn;
    private IntVec3 HookedNodeCell = IntVec3.Invalid;

    private int lastPwnMishlapTick = -99999;

    private int lastUsedTick = -99999;

    private float WorkDone;

    public CompProperties_MineShaft PropsMine => (CompProperties_MineShaft)props;

    private OreNode NodePointer
    {
        get
        {
            foreach (var getNode in parent.Map.GetComponent<OreMapComponent>().GetNodes)
            {
                foreach (var cell in getNode.Cells)
                {
                    if (cell == HookedNodeCell)
                    {
                        return getNode;
                    }
                }
            }

            return null;
        }
    }

    private void HookToNode()
    {
        var num = 1000f;
        foreach (var getNode in parent.Map.GetComponent<OreMapComponent>().GetNodes)
        {
            foreach (var cell in getNode.Cells)
            {
                if (!cell.InHorDistOf(parent.Position, MiningRadius) || !(cell.DistanceTo(parent.Position) < num))
                {
                    continue;
                }

                cell.DistanceTo(parent.Position);
                HookedNodeCell = cell;
                return;
            }
        }

        HookedNodeCell = IntVec3.Invalid;
    }

    public override void PostPostMake()
    {
        base.PostPostMake();
        FirstSpawn = true;
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (!FirstSpawn)
        {
            return;
        }

        if (parent.Map == null)
        {
            Log.Message("Map still is null");
        }

        HookToNode();
        if (HookedNodeCell == IntVec3.Invalid || !(NodePointer.OreAmountGet <= 0f))
        {
            return;
        }

        parent.Map?.GetComponent<OreMapComponent>().RemoveNode(NodePointer);
        HookToNode();
    }

    public void MiningWorkDone(Pawn pawn)
    {
        WorkDone += baseWorkPerCycle * pawn.GetStatValue(StatDefOf.MiningSpeed);
        if (!(WorkDone >= WorkNeededForNextPortion))
        {
            return;
        }

        WorkDone -= WorkNeededForNextPortion;
        if (lastPwnMishlapTick + (OreSettingsHelper.ModSettings.AccidentIntervalDays * ticksInDay) < lastUsedTick)
        {
            lastPwnMishlapTick = Find.TickManager.TicksGame;
            var statValue = pawn.GetStatValue(OreDefOf.MiningMistakeChance);
            if (Rand.Value < statValue)
            {
                PawnMiningAccident(pawn);
            }
        }

        if (HookedNodeCell != IntVec3.Invalid && NodePointer != null)
        {
            var num = Mathf.Min((int)NodePointer.OreAmountGet,
                (int)(pawn.GetStatValue(StatDefOf.MiningYield) * NodePointer.ResourceDef.deepCountPerPortion));
            var thing = ThingMaker.MakeThing(NodePointer.ResourceDef);
            thing.stackCount = num;
            GenPlace.TryPlaceThing(thing, parent.InteractionCell, parent.Map, ThingPlaceMode.Near);
            NodePointer.MineChunk(num);
            if (!(NodePointer.OreAmountGet <= 0f))
            {
                return;
            }

            parent.Map.GetComponent<OreMapComponent>().RemoveNode(NodePointer);
            HookToNode();
        }
        else
        {
            TryProduceRock();
            HookToNode();
        }
    }

    private void TryProduceRock()
    {
        var def = (from rock in Find.World.NaturalRockTypesIn(parent.Map.Tile)
            select rock.building.mineableThing).FirstOrDefault();
        var thing = ThingMaker.MakeThing(def);
        thing.stackCount = 1;
        GenPlace.TryPlaceThing(thing, parent.InteractionCell, parent.Map, ThingPlaceMode.Near);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref HookedNodeCell, "HookedCell", IntVec3.Invalid);
        Scribe_Values.Look(ref lastUsedTick, "lastusedtick", -999999);
        Scribe_Values.Look(ref lastPwnMishlapTick, "lastwypadektick", -999999);
        Scribe_Values.Look(ref WorkDone, "WorkDone");
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }
    }

    public override string CompInspectStringExtra()
    {
        var empty = base.CompInspectStringExtra();
        var f = WorkDone / WorkNeededForNextPortion;
        empty = $"{empty}Progress to new portion: {f.ToStringPercent()}\n";
        if (NodePointer == null)
        {
            return empty + (NodePointer == null ? "There is no node hooked" : "");
        }

        empty += "Ore Level:";
        empty += (int)NodePointer.OreAmountGet;

        return empty + (NodePointer == null ? "There is no node hooked" : "");
    }

    public bool CanMine()
    {
        return true;
    }

    private static void TryAddToCrushedThingsList(Thing t, List<Thing> outCrushedThings)
    {
        if (outCrushedThings != null && !outCrushedThings.Contains(t) && WorthMentioningInCrushLetter(t))
        {
            outCrushedThings.Add(t);
        }
    }

    private static bool WorthMentioningInCrushLetter(Thing t)
    {
        if (!t.def.destroyable)
        {
            return false;
        }

        return t.def.category switch
        {
            ThingCategory.Building => true,
            ThingCategory.Pawn => true,
            ThingCategory.Item => t.MarketValue > 0.01f,
            _ => false
        };
    }

    private void PawnMiningAccident(Pawn miner)
    {
        if (miner == null)
        {
            return;
        }

        var num = (int)(DamageCountRange.start + (DamageCountRange.length * Rand.Value));
        var num2 = AverageDamage * DamageRandomFactorRange.RandomInRange;
        var num3 = num2 / num;
        var num4 = num3 * 0.0015f;
        for (var i = 0; i < (float)num; i++)
        {
            var dinfo = new DamageInfo(DamageDefOf.Blunt, num3, num4);
            var damageResult = miner.TakeDamage(dinfo);
            if (i != 0)
            {
                continue;
            }

            var battleLogEntry_DamageTaken =
                new BattleLogEntry_DamageTaken(miner, RulePackDefOf.DamageEvent_TrapSpike);
            Find.BattleLog.Add(battleLogEntry_DamageTaken);
            damageResult.AssociateWithLog(battleLogEntry_DamageTaken);
        }

        if (Rand.Value < 0.05)
        {
            var list = new List<Thing>();
            TryAddToCrushedThingsList(miner, list);
            var crush = DamageDefOf.Crush;
            var num5 = 99999f;
            num4 = 999f;
            var brain = miner.health.hediffSet.GetBrain();
            var dinfo = new DamageInfo(crush, num5, num4, -1f, null, brain, null, DamageInfo.SourceCategory.Collapse);
            var battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(miner, RulePackDefOf.DamageEvent_Ceiling);
            Find.BattleLog.Add(battleLogEntry_DamageTaken);
            miner.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_DamageTaken);
            if (!miner.Destroyed && miner.def.destroyable)
            {
                miner.Kill(new DamageInfo(DamageDefOf.Crush, 99999f, 999f, -1f, null, null, null,
                    DamageInfo.SourceCategory.Collapse));
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("RoofCollapsed".Translate());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("TheseThingsCrushed".Translate());
            var list2 = new List<string>();
            foreach (var thing in list)
            {
                var item = thing.LabelShort.CapitalizeFirst();
                if (thing.def.category == ThingCategory.Pawn)
                {
                    item = thing.LabelCap;
                }

                if (!list2.Contains(item))
                {
                    list2.Add(item);
                }
            }

            foreach (var item2 in list2)
            {
                stringBuilder.AppendLine($"    -{item2}");
            }

            Find.LetterStack.ReceiveLetter("LetterLabelRoofCollapsed".Translate(),
                stringBuilder.ToString().TrimEndNewlines(), LetterDefOf.NegativeEvent,
                new TargetInfo(parent.Position, parent.Map));
        }

        Messages.Message("Mine collapsed on colonist!", new TargetInfo(parent.InteractionCell, parent.Map),
            MessageTypeDefOf.NegativeEvent);
        SoundDefOf.Roof_Collapse.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        base.PostDrawExtraSelectionOverlays();
        GenDraw.DrawRadiusRing(parent.Position, MiningRadius);
    }
}