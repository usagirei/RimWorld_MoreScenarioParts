﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{

    public class ForcedHediffModifier : ScenPartEx_PawnModifier
    {
        private HediffDef hediff;
        private FloatRange severityRange;
        private bool hideOffMap;

        private float getMaxSeverity(HediffDef def)
        {
            return def.lethalSeverity > 0f
                ? (def.lethalSeverity * 0.99f)
                : 1f;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<HediffDef>(ref hediff, nameof(hediff));
            Scribe_Values.Look<FloatRange>(ref severityRange, nameof(severityRange));
            Scribe_Values.Look<bool>(ref hideOffMap, nameof(hideOffMap));
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 5 + 31f);
            Rect[] rows = rect.SplitRows(RowHeight, 31f, 4 * RowHeight);

            if (Widgets.ButtonText(rows[0], this.hediff.LabelCap, true, false, true))
            {
                FloatMenuUtility.MakeMenu(DefDatabase<HediffDef>.AllDefs.Where((HediffDef x) => x.scenarioCanAdd), (x) => x.LabelCap, (x) => () =>
                {
                    hediff = x;
                    var maxSeverity = getMaxSeverity(x);
                    if (severityRange.max > maxSeverity)
                        severityRange.max = maxSeverity;
                    if (severityRange.min > maxSeverity)
                        severityRange.min = maxSeverity;
                });
            }

            Widgets.FloatRange(rows[1], listing.CurHeight.GetHashCode(), ref severityRange, 0, getMaxSeverity(hediff), "ConfigurableSeverity");
            DoContextEditInterface(rows[2]);

            if (context == PawnModifierContext.PlayerStarter)
            {
                Rect r = rows[2].BottomPart(0.25f);
                Widgets.CheckboxLabeled(r, "starting only (defered)", ref hideOffMap);
            }
        }


        public override void Randomize()
        {
            base.Randomize();
            hediff = DefDatabase<HediffDef>.AllDefs.Where((HediffDef x) => x.scenarioCanAdd).RandomElement();
            severityRange.max = Rand.Range(getMaxSeverity(hediff) * 0.2f, getMaxSeverity(hediff) * 0.95f);
            severityRange.min = severityRange.max * Rand.Range(0f, 0.95f);
        }

        public override bool TryMerge(ScenPart other)
        {
            if (other is ForcedHediffModifier fhm && this.hediff == fhm.hediff)
            {
                chance = GenMath.ChanceEitherHappens(this.chance, fhm.chance);
                return true;
            }
            return false;
        }

        protected override void ModifyNewPawn(Pawn p, bool humanLike)
        {
            if (hideOffMap && context == PawnModifierContext.PlayerStarter)
                return;

            Hediff hediff = HediffMaker.MakeHediff(this.hediff, p, null);
            hediff.Severity = severityRange.RandomInRange;
            p.health.AddHediff(hediff, null, null, null);

        }

        public override void PostMapGenerate(Map map)
        {
            if (!hideOffMap || Find.GameInitData == null || context != PawnModifierContext.PlayerStarter)
                return;

            foreach (Pawn p in Find.GameInitData.startingAndOptionalPawns)
            {
                if (Rand.Chance(this.chance) && p.RaceProps.Humanlike)
                {
                    this.ModifyNewPawn(p, p.RaceProps.Humanlike);
                }
            }
        }

        public override bool AllowPlayerStartingPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return base.AllowPlayerStartingPawn(pawn, tryingToRedress, req) || DisallowIfWouldDie(pawn, req);
        }

        public override bool AllowWorldGeneratedPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return base.AllowPlayerStartingPawn(pawn, tryingToRedress, req) && DisallowIfWouldDie(pawn, req);
        }

        private bool DisallowIfWouldDie(Pawn pawn, PawnGenerationRequest req)
        {
            if (!req.AllowDead && pawn.health.WouldDieAfterAddingHediff(hediff, null, severityRange.max))
                return false;
            if (!req.AllowDowned && pawn.health.WouldBeDownedAfterAddingHediff(hediff, null, severityRange.max))
                return false;
            return true;
        }
    }
}