using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class ForcedTraitModifier : ScenPartEx_PawnModifier
    {
        private int degree;
        private TraitDef trait;

        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 5);
            Rect[] rows = rect.SplitRows(1, 4);

            if (Widgets.ButtonText(rows[0], trait.DataAtDegree(degree).label.CapitalizeFirst(), true, false, true))
            {
                var defs = from def in DefDatabase<TraitDef>.AllDefs
                           from deg in def.degreeDatas
                           orderby def.label ascending
                           select new { Trait = def, Degree = deg };

                FloatMenuUtility.MakeMenu(defs, (x) => x.Degree.label.CapitalizeFirst(), (x) => () =>
                {
                    trait = x.Trait;
                    degree = x.Degree.degree;
                });
            }
            DoContextEditInterface(rows[1]);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref trait, nameof(trait));
            Scribe_Values.Look(ref degree, nameof(degree), 0, false);
        }

        public override void Randomize()
        {
            base.Randomize();
            trait = DefDatabase<TraitDef>.GetRandom();
            degree = trait.degreeDatas.RandomElement().degree;
        }

        protected override void ModifyGeneratedPawn(Pawn pawn, bool redressed, bool humanLike)
        {
            if (!humanLike)
            {
                return;
            }

            if (pawn.story == null || pawn.story.traits == null)
            {
                return;
            }

            if (pawn.story.traits.HasTrait(trait) && pawn.story.traits.DegreeOfTrait(trait) == degree)
            {
                return;
            }

            if (pawn.story.traits.HasTrait(trait))
            {
                pawn.story.traits.allTraits.RemoveAll((Trait tr) => tr.def == trait);
            }
            else
            {
                IEnumerable<Trait> traitsNotForced = pawn.story.traits.allTraits.Where((Trait tr) => !(tr.ScenForced || PawnHasTraitForcedByBackstory(pawn, tr.def)));
                if (traitsNotForced.Any())
                {
                    Trait conflictingTrait = traitsNotForced.Where((Trait tr) => tr.def.conflictingTraits.Contains(trait)).FirstOrDefault();
                    if (conflictingTrait != null)
                    {
                        pawn.story.traits.allTraits.Remove(conflictingTrait);
                    }
                    else
                    {
                        pawn.story.traits.allTraits.Remove(traitsNotForced.RandomElement());
                    }
                }
            }
            pawn.story.traits.GainTrait(new Trait(trait, degree, true));
        }

        private static bool PawnHasTraitForcedByBackstory(Pawn pawn, TraitDef trait)
        {
            if (pawn.story.childhood != null
                && pawn.story.childhood.forcedTraits != null
                && pawn.story.childhood.forcedTraits.Any((TraitEntry te) => te.def == trait))
            {
                return true;
            }

            if (pawn.story.adulthood != null
                && pawn.story.adulthood.forcedTraits != null
                && pawn.story.adulthood.forcedTraits.Any((TraitEntry te) => te.def == trait))
            {
                return true;
            }

            return false;
        }
    }
}
