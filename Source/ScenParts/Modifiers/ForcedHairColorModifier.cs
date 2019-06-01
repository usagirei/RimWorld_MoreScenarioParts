using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{

    public class ForcedHairColorModifier : ScenPartEx_PawnModifier
    {
        private FloatRange hairColorR;
        private FloatRange hairColorG;
        private FloatRange hairColorB;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<FloatRange>(ref hairColorR, nameof(hairColorR));
            Scribe_Values.Look<FloatRange>(ref hairColorG, nameof(hairColorG));
            Scribe_Values.Look<FloatRange>(ref hairColorB, nameof(hairColorB));
        }

        private float phase = 0;
        private float step = (360f / 5) * Mathf.Deg2Rad;
        private float offset = Mathf.Sin(180 / 3 * Mathf.Deg2Rad);
        private Color curCol;

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            phase += Time.deltaTime * step;

            float r = (Mathf.Sin(phase * 0.75f + 0 * offset) + 1) / 2f;
            float g = (Mathf.Sin(phase * 1.00f + 1 * offset) + 1) / 2f;
            float b = (Mathf.Sin(phase * 1.25f + 2 * offset) + 1) / 2f;

            r = hairColorR.LerpThroughRange(r);
            g = hairColorG.LerpThroughRange(g);
            b = hairColorB.LerpThroughRange(b);

            curCol = new Color(r, g, b);

            Rect rect = listing.GetScenPartRect(this, RowHeight * 4 + 3 * 31f);
            Rect[] rows = rect.SplitRows( 31f * 3, 4 * RowHeight );

            Rect[] cols = rows[0].SplitCols( rows[0].width - rows[0].height, rows[0].height );
            Rect prev = cols[1].ContractedBy(8);
            Rect[] sliders = cols[0].SplitRows(1, 1, 1);

            Widgets.FloatRange(sliders[0], listing.CurHeight.GetHashCode() + 0, ref hairColorR);
            Widgets.FloatRange(sliders[1], listing.CurHeight.GetHashCode() + 1, ref hairColorG);
            Widgets.FloatRange(sliders[2], listing.CurHeight.GetHashCode() + 2, ref hairColorB);
            
            Widgets.DrawBoxSolid(prev, curCol);

            Log.Message(curCol.ToStringSafe());

            DoContextEditInterface(rows[1]);
        }

        public override void Randomize()
        {
            FloatRange randRang()
            {
                var r = new FloatRange(0, 1);
                switch (Rand.RangeInclusive(0, 2))
                {
                    case 0:
                        r.min = Rand.Range(0, 1);
                        break;
                    case 1:
                        r.max = Rand.Range(0, 1);
                        break;
                    case 2:
                        r.min = Rand.Range(0, 1);
                        r.max = Rand.Range(0, 1);
                        break;
                }

                if (r.max < r.min)
                {
                    r.min = r.max;
                }

                return r;
            }

            base.Randomize();

            hairColorR = randRang();
            hairColorG = randRang();
            hairColorB = randRang();
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

            pawn.story.hairColor = new Color(hairColorR.RandomInRange, hairColorG.RandomInRange, hairColorB.RandomInRange);
        }
    }
}