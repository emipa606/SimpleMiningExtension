using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace oreprocessing;

public class JobDriverProspecting : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        _ = job.targetB.Cell;
        var Comp = job.targetA.Thing.TryGetComp<PrimitiveOreRadarComp>();
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Deconstruct);
        this.FailOn(() => Comp.canSeeOverlay);
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell);
        yield return Toils_General
            .WaitWith(TargetIndex.B, (int)(4000f / pawn.GetStatValue(StatDefOf.ResearchSpeed)), true)
            .WithEffect(EffecterDefOf.ConstructDirt, TargetIndex.B);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
        yield return Toils_General.WaitWith(TargetIndex.A, 500, true)
            .WithEffect(EffecterDefOf.ConstructDirt, TargetIndex.A);
        yield return ProspectBuilding();
        yield return SpawnFilth();
    }

    private Toil ProspectBuilding()
    {
        return new Toil
        {
            initAction = delegate
            {
                var building = (Building)pawn.CurJob.targetA.Thing;
                var comp = building.GetComp<PrimitiveOreRadarComp>();
                comp.WorkAtProspectSite(pawn);
            }
        };
    }

    private Toil SpawnFilth()
    {
        return new Toil
        {
            initAction = delegate
            {
                var thing = ThingMaker.MakeThing(ThingDefOf.Filth_Dirt);
                var thing2 = ThingMaker.MakeThing(ThingDefOf.Filth_RubbleRock);
                GenPlace.TryPlaceThing(thing, job.targetA.Cell, job.targetA.Thing.Map, ThingPlaceMode.Near);
                GenPlace.TryPlaceThing(thing2, job.targetA.Cell, job.targetA.Thing.Map, ThingPlaceMode.Near);
            }
        };
    }
}