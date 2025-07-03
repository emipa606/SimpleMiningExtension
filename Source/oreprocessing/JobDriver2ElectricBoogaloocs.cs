using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace oreprocessing;

internal class JobDriver2ElectricBoogaloocs : JobDriver
{
    public const int BaseTicksBetweenPickHits = 100;

    private Effecter effecter;
    private int ticksToPickHit = -1000;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var compMiningPlatform = job.targetA.Thing.TryGetComp<CompMineShaft>();
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Deconstruct);
        this.FailOn(() => !CompMineShaft.CanMine());
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        yield return mineStuffFromMine();
        yield return mineShaftYieldStuff();
        yield return applyHeDiff();
    }

    private Toil mineStuffFromMine()
    {
        return new Toil
        {
            tickAction = delegate
            {
                var building = (Building)pawn.CurJob.targetA.Thing;
                building.GetComp<CompMineShaft>();
                if (ticksToPickHit < -100)
                {
                    resetTicksToPickHit();
                }

                pawn.skills?.Learn(SkillDefOf.Mining, 0.11f);

                ticksToPickHit--;
                if (ticksToPickHit > 0)
                {
                    return;
                }

                effecter ??= EffecterDefOf.Mine.Spawn();

                effecter.Trigger(pawn, building);
                resetTicksToPickHit();
            },
            defaultDuration =
                (int)Mathf.Clamp(OreSettingsHelper.ModSettings.WorkDuration / pawn.GetStatValue(StatDefOf.MiningSpeed),
                    800f, 7000f),
            defaultCompleteMode = ToilCompleteMode.Delay,
            handlingFacing = true
        }.WithProgressBarToilDelay(TargetIndex.B);
    }

    private Toil mineShaftYieldStuff()
    {
        return new Toil
        {
            initAction = delegate
            {
                var building = (Building)pawn.CurJob.targetA.Thing;
                var comp = building.GetComp<CompMineShaft>();
                comp.MiningWorkDone(pawn);
            }
        };
    }

    private Toil applyHeDiff()
    {
        return new Toil
        {
            initAction = delegate
            {
                var hediff = pawn?.health?.hediffSet?.GetFirstHediffOfDef(OreDefOf.MinersHunger);
                if (hediff == null)
                {
                    var hediff2 = HediffMaker.MakeHediff(OreDefOf.MinersHunger, pawn);
                    hediff2.Severity = 0.25f;
                    pawn?.health?.AddHediff(hediff2);
                }
                else
                {
                    hediff.Severity += 0.25f;
                }
            }
        };
    }

    private void resetTicksToPickHit()
    {
        var num = pawn.GetStatValue(StatDefOf.MiningSpeed);
        if (num < 0.6f && pawn.Faction != Faction.OfPlayer)
        {
            num = 0.6f;
        }

        ticksToPickHit = (int)Math.Round(100f / num);
    }
}