using RimWorld;
using Verse;
using Verse.AI;

namespace oreprocessing;

public class WorkGiver_MiningOperation : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(OreDefOf.MiningPlatform);

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        var allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
        foreach (var building in allBuildingsColonist)
        {
            if (building.def != OreDefOf.MiningPlatform)
            {
                continue;
            }

            var comp = building.GetComp<CompPowerTrader>();
            if ((comp == null || comp.PowerOn) &&
                building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) == null)
            {
                return false;
            }
        }

        return true;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (!(t is Building building))
        {
            return false;
        }

        if (building.IsForbidden(pawn))
        {
            return false;
        }

        LocalTargetInfo target = building;
        if (!pawn.CanReserve(target, 1, -1, null, forced))
        {
            return false;
        }

        var compMineShaft = building.TryGetComp<CompMineShaft>();
        return compMineShaft.CanMine() &&
               building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) == null &&
               !building.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(OreDefOf.MineAtPlatform, t);
    }
}