using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

using RString = R.MoreScenarioParts.String;

namespace More_Scenario_Parts.ScenParts
{

    public class SetNeedLevelModifier : ScenPartEx_PawnModifier
    {
        private NeedDef need;
        private FloatRange levelRange;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref need, nameof(need));
            Scribe_Values.Look(ref levelRange, nameof(levelRange));
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 5 + 31f);
            Rect[] rows = rect.SplitRows(RowHeight, 31f, 4 * RowHeight);

            if (Widgets.ButtonText(rows[0], this.need.LabelCap, true, false, true))
            {
                FloatMenuUtility.MakeMenu(DefDatabase<NeedDef>.AllDefs.Where((NeedDef x) => x.major), (x) => x.LabelCap, (x) => () => need = x);
            }

            Widgets.FloatRange(rows[1], listing.CurHeight.GetHashCode(), ref levelRange, 0, 1, RString.Keys.MSP_NeedLevel);
            DoContextEditInterface(rows[2]);
        }

        public override void Randomize()
        {
            base.Randomize();
            this.need = DefDatabase<NeedDef>.AllDefs.Where((NeedDef x) => x.major).RandomElement();
            this.levelRange.max = Rand.Range(0f, 1f);
            this.levelRange.min = this.levelRange.max * Rand.Range(0f, 0.95f);
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }

        protected override void ModifyGeneratedPawn(Pawn p, bool redressed, bool humanLike)
        {
            if (!humanLike)
                return;

            if (p.needs == null)
                return;

            Need need = p.needs.TryGetNeed(this.need);
            if (need == null)
                return;

            need.CurLevelPercentage = levelRange.RandomInRange;
        }
    }
}