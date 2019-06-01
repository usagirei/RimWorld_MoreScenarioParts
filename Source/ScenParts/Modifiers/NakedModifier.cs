using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{

    public class NakedModifier : ScenPartEx_PawnModifier
    {
        private TraitDef trait;
        private int degree;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<TraitDef>(ref trait, "trait");
            Scribe_Values.Look<int>(ref degree, "degree", 0, false);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 4);
            DoContextEditInterface(rect);
        }

        public override void Randomize()
        {
            base.Randomize();
            this.trait = DefDatabase<TraitDef>.GetRandom();
            this.degree = this.trait.degreeDatas.RandomElement().degree;
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            return true;
        }

        protected override void ModifyGeneratedPawn(Pawn pawn, bool redressed, bool humanLike)
        {
            if (!humanLike)
                return;

            if (pawn.apparel != null)
            {
                pawn.apparel.DestroyAll(DestroyMode.Vanish);
            }
        }
    }
}