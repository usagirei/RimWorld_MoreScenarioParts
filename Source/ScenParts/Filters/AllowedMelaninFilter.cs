using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{

    public class AllowedMelaninFilter : ScenPartEx_PawnFilter
    {
        private FloatRange allowedMelaninRange;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref allowedMelaninRange, nameof(allowedMelaninRange));
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 4);
            Rect[] rows = rect.SplitRows(1, 3);

            Widgets.FloatRange(rows[0], listing.CurHeight.GetHashCode(), ref allowedMelaninRange);
            FixMelaninRange();

            DoContextEditInterface(rows[1]);
        }

        public override void Randomize()
        {
            base.Randomize();

            allowedMelaninRange = new FloatRange(0, 1);
            switch (Rand.RangeInclusive(0, 2))
            {
                case 0:
                    allowedMelaninRange.min = Rand.Range(0, 1);
                    break;
                case 1:
                    allowedMelaninRange.max = Rand.Range(0, 1);
                    break;
                case 2:
                    allowedMelaninRange.min = Rand.Range(0, 1);
                    allowedMelaninRange.max = Rand.Range(0, 1);
                    break;
            }

            FixMelaninRange();
        }

        private void FixMelaninRange()
        {
            if (allowedMelaninRange.max - allowedMelaninRange.min < 0.2)
            {
                allowedMelaninRange.min = allowedMelaninRange.max - 0.2f;
            }
            if (allowedMelaninRange.min < 0)
            {
                allowedMelaninRange.max += -allowedMelaninRange.min;
                allowedMelaninRange.min = 0;
            }
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }

        public override bool AllowPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return allowedMelaninRange.Includes(pawn.story.melanin);
        }


    }
}