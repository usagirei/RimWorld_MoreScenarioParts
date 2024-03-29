﻿using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class SetNeedLevelModifier : ScenPartEx_PawnModifier
    {
        private FloatRange levelRange;
        private NeedDef need;

        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 5 + 31f);
            Rect[] rows = rect.SplitRows(RowHeight, 31f, 4 * RowHeight);

            if (Widgets.ButtonText(rows[0], need.LabelCap, true, false, true))
            {
                FloatMenuUtility.MakeMenu(DefDatabase<NeedDef>.AllDefs.Where((NeedDef x) => x.major), (x) => x.LabelCap, (x) => () => need = x);
            }

            Widgets.FloatRange(rows[1], listing.CurHeight.GetHashCode(), ref levelRange, 0, 1, R.String.Keys.MSP_NeedLevel.CapitalizeFirst());
            DoContextEditInterface(rows[2]);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref need, nameof(need));
            Scribe_Values.Look(ref levelRange, nameof(levelRange));
        }

        public override void Randomize()
        {
            base.Randomize();
            need = DefDatabase<NeedDef>.AllDefs.Where((NeedDef x) => x.major).RandomElement();
            levelRange.max = Rand.Range(0f, 1f);
            levelRange.min = levelRange.max * Rand.Range(0f, 0.95f);
        }

        protected override void ModifyGeneratedPawn(Pawn pawn, bool redressed, bool humanLike)
        {
            if (!humanLike)
            {
                return;
            }

            if (pawn.needs == null)
            {
                return;
            }

            Need pawnNeed = pawn.needs.TryGetNeed(need);
            if (pawnNeed == null)
            {
                return;
            }

            pawnNeed.CurLevelPercentage = levelRange.RandomInRange;
        }
    }
}
