using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{

    public class AllowedAgeFilter : ScenPartEx_PawnFilter
    {
        private IntRange allowedAgeRange;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref allowedAgeRange, nameof(allowedAgeRange));
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 4);
            Rect[] rows = rect.SplitRows(1, 3);

            Widgets.IntRange(rows[0], listing.CurHeight.GetHashCode(), ref allowedAgeRange, 15, 120);
            FixAgeRange();

            DoContextEditInterface(rows[1]);
        }

        public override void Randomize()
        {
            base.Randomize();

            allowedAgeRange = new IntRange(15, 120);
            switch (Rand.RangeInclusive(0, 2))
            {
                case 0:
                    allowedAgeRange.min = Rand.Range(20, 60);
                    break;
                case 1:
                    allowedAgeRange.max = Rand.Range(20, 60);
                    break;
                case 2:
                    allowedAgeRange.min = Rand.Range(20, 60);
                    allowedAgeRange.max = Rand.Range(20, 60);
                    break;
            }

            FixAgeRange();
        }

        private void FixAgeRange()
        {
            if (allowedAgeRange.max < 19)
            {
                allowedAgeRange.max = 19;
            }
            if (allowedAgeRange.max - allowedAgeRange.min < 4)
            {
                allowedAgeRange.min = allowedAgeRange.max - 4;
            }
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }

        public override bool AllowPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return allowedAgeRange.Includes(pawn.ageTracker.AgeBiologicalYears);
        }


    }
}