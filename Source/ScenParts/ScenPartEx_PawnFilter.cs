using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

using StringResource = R.MoreScenarioParts.String;

namespace More_Scenario_Parts.ScenParts
{
    public class ScenPartEx_PawnFilter : ScenPartEx
    {
        protected PawnModifierContext context;
        protected PawnModifierGender gender;
        protected FactionDef faction;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<PawnModifierContext>(ref this.context, nameof(context), PawnModifierContext.All, false);
            Scribe_Values.Look<FactionDef>(ref faction, nameof(faction));
            Scribe_Values.Look<PawnModifierGender>(ref gender, nameof(gender));

        }

        protected void DoContextEditInterface(Rect rect)
        {
            bool isFaction = context == PawnModifierContext.Faction;

            Rect[] rows = rect.SplitRows(1, 1, 1);

            Rect[] rect_gender = rows[0].SplitCols(1, 2);
            Rect[] rect_context = rows[1].SplitCols(1, 2);
            Rect[] rect_faction = rows[2].SplitCols(1, 2);

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_gender[0], StringResource.MSP_Gender);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_gender[1], gender.Translate()))
            {
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<PawnModifierGender>(), ScenPartExtensions.Translate, (g) => () => gender = g);
            }

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_context[0], StringResource.MSP_Context);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_context[1], context.Translate(), true, false, true))
            {
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<PawnModifierContext>(), ScenPartExtensions.Translate, (c) => () => context = c);
            }

            if (isFaction)
            {
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(rect_faction[0], StringResource.MSP_Faction);
                Text.Anchor = TextAnchor.UpperLeft;

                if (Widgets.ButtonText(rect_faction[1], faction.LabelCap, true, false, true))
                {
                    FloatMenuUtility.MakeMenu(DefDatabase<FactionDef>.AllDefs.Where((d) => !d.isPlayer), (f) => f.LabelCap, (f) => () => faction = f);
                }
            }
        }

        public override void Randomize()
        {
            context = Utilities.GetEnumValues<PawnModifierContext>().RandomElement();
            faction = faction = DefDatabase<FactionDef>.AllDefs.Where(f => !f.isPlayer).RandomElement();
            gender = Utilities.GetEnumValues<PawnModifierGender>().RandomElement();
        }

        public sealed override bool AllowPlayerStartingPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            if (!gender.Includes(pawn.gender))
                return true;

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = req.Context == PawnGenerationContext.PlayerStarter;

            switch (context)
            {
                case PawnModifierContext.PlayerStarter when !isStartingPawn:
                    return true;
                case PawnModifierContext.PlayerNonStarter when isStartingPawn:
                    return true;

                case PawnModifierContext.Player when !isPlayerFaction:
                    return true;
                case PawnModifierContext.NonPlayer when isPlayerFaction:
                    return true;                

                case PawnModifierContext.Faction when pawn.Faction.def != faction:
                    return true;
            }

            return AllowPawn(pawn, tryingToRedress, req);
        }

        public sealed override bool AllowWorldGeneratedPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            if (!gender.Includes(pawn.gender))
                return true;

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = req.Context == PawnGenerationContext.PlayerStarter;

            switch (context)
            {
                case PawnModifierContext.PlayerStarter when !isStartingPawn:
                    return true;
                case PawnModifierContext.PlayerNonStarter when isStartingPawn:
                    return true;

                case PawnModifierContext.Player when !isPlayerFaction:
                    return true;
                case PawnModifierContext.NonPlayer when isPlayerFaction:
                    return true;

                case PawnModifierContext.Faction when pawn.Faction.def != faction:
                    return true;
            }

            return AllowPawn(pawn, tryingToRedress, req);
        }

        public virtual bool AllowPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return true;
        }
    }
}
