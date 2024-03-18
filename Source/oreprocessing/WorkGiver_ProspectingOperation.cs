using RimWorld;
using Verse;
using Verse.AI;

namespace oreprocessing;

public class WorkGiver_ProspectingOperation : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(OreDefOf.OrePrimitiveRadar);

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
            if (building.def != OreDefOf.OrePrimitiveRadar)
            {
                continue;
            }

            var comp = building.GetComp<CompPowerTrader>();
            if ((comp == null || comp.PowerOn) &&
                building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) == null)
            {
                return false;
            }

            var comp2 = building.GetComp<PrimitiveOreRadarComp>();
            if (comp2.canSeeOverlay)
            {
                return true;
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

        if (t is not Building building)
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

        var primitiveOreRadarComp = building.TryGetComp<PrimitiveOreRadarComp>();
        return !primitiveOreRadarComp.canSeeOverlay &&
               building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) == null &&
               !building.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var primitiveOreRadarComp = t.TryGetComp<PrimitiveOreRadarComp>();
        var intVec = primitiveOreRadarComp.ReturnValidCellToDig(pawn);
        return new Job(OreDefOf.SearchForResources, t, intVec);
    }
}