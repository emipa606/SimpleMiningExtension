using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace oreprocessing;

public class JobDriverOreMine : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Deconstruct);
        this.FailOn(delegate
        {
            var compMineShaft = job.targetA.Thing.TryGetComp<CompMineShaft>();
            return !CompMineShaft.CanMine();
        });
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        var work = new Toil();
        work.tickAction = delegate
        {
            var actor = work.actor;
            var building = (Building)actor.CurJob.targetA.Thing;
            building.GetComp<CompMineShaft>();
            var hediff = work.actor?.health?.hediffSet?.GetFirstHediffOfDef(OreDefOf.MinersHunger);
            if (hediff == null)
            {
                var hediff2 = HediffMaker.MakeHediff(OreDefOf.MinersHunger, work.actor);
                hediff2.Severity = 0.01f;
                work.actor?.health?.AddHediff(hediff2);
            }
            else
            {
                hediff.Severity += 0.01f;
            }

            actor.skills.Learn(SkillDefOf.Mining, 0.065f);
        };
        work.defaultCompleteMode = ToilCompleteMode.Never;
        work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
        work.WithEffect(EffecterDefOf.Mine, TargetIndex.B);
        work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
        work.activeSkill = () => SkillDefOf.Mining;
        yield return work;
    }
}