using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{

    public class ForcedMelaninModifier : ScenPartEx_PawnModifier
    {
        private FloatRange melanin;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref melanin, nameof(melanin));
        }

        private float phase = 0;
        private float step = (360f / 5) * Mathf.Deg2Rad;
        private Color curCol;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            phase += Time.deltaTime * step;

            float t = (Mathf.Sin(phase) + 1) / 2f;

            curCol = PawnSkinColors.GetSkinColor(melanin.LerpThroughRange(t));

            Rect rect = listing.GetScenPartRect(this, RowHeight * 4 + 31f);
            Rect[] rows = rect.SplitRows(31f, 4 * RowHeight);

            Rect[] cols = rows[0].SplitCols(rows[0].width - 31f, 31f);


            Widgets.FloatRange(cols[0], listing.CurHeight.GetHashCode(), ref melanin);
            Widgets.DrawBoxSolid(cols[1].ContractedBy(4), curCol);
            FixMelaninRange();

            DoContextEditInterface(rows[1]);
        }

        public override void Randomize()
        {
            base.Randomize();

            melanin = new FloatRange(0, 1);
            switch (Rand.RangeInclusive(0, 2))
            {
                case 0:
                    melanin.min = Rand.Range(0, 1);
                    break;
                case 1:
                    melanin.max = Rand.Range(0, 1);
                    break;
                case 2:
                    melanin.min = Rand.Range(0, 1);
                    melanin.max = Rand.Range(0, 1);
                    break;
            }

            FixMelaninRange();
        }

        private void FixMelaninRange()
        {
            if (melanin.min < 0)
            {
                melanin.min = 0;
            }
            if (melanin.max < melanin.min)
            {
                melanin.min = melanin.max;
            }
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }

        protected override void ModifyGeneratedPawn(Pawn pawn, bool redressed, bool humanLike)
        {
            if (!humanLike)
            {
                return;
            }

            if (pawn.story == null)
            {
                return;
            }

            pawn.story.melanin = melanin.RandomInRange;
        }
    }
}