using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class NakedModifier : ScenPartEx_PawnModifier
    {
        public override bool CanCoexistWith(ScenPart other)
        {
            return true;
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 4);
            DoContextEditInterface(rect);
        }

        protected override void ModifyGeneratedPawn(Pawn pawn, bool redressed, bool humanLike)
        {
            if (!humanLike)
            {
                return;
            }

            if (pawn.apparel != null)
            {
                pawn.apparel.DestroyAll(DestroyMode.Vanish);
            }
        }
    }
}
