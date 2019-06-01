using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

using RString = R.MoreScenarioParts.String;

namespace More_Scenario_Parts.ScenParts
{

    public class ForcedGenderModifier : ScenPartEx
    {
        private float chance = 1f;
        private PawnModifierContext context;
        private PawnModifierGender gender;
        private FactionDef faction;

        private string chanceBuf;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref chance, nameof(chance), 0f, false);
            Scribe_Values.Look<PawnModifierContext>(ref this.context, nameof(context), PawnModifierContext.All, false);
            Scribe_Values.Look<FactionDef>(ref faction, nameof(faction));
            Scribe_Values.Look<PawnModifierGender>(ref gender, nameof(gender));
        }


        public override void Randomize()
        {
            base.Randomize();
            gender = Rand.Bool ? PawnModifierGender.Male : PawnModifierGender.Female;
            chance = GenMath.RoundedHundredth(Rand.Range(0.05f, 1f));
            context = Utilities.GetEnumValues<PawnModifierContext>().RandomElement();
            faction = faction = DefDatabase<FactionDef>.AllDefs.Where(f => !f.isPlayer).RandomElement();
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 4);

            bool isFaction = context == PawnModifierContext.Faction;

            Rect[] rows = rect.SplitRows(1, 1, 1, 1);

            Rect[] rect_gender = rows[0].SplitCols(1, 2);
            Rect[] rect_chance = rows[1].SplitCols(1, 2);
            Rect[] rect_context = rows[2].SplitCols(1, 2);
            Rect[] rect_faction = rows[3].SplitCols(1, 2);

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_chance[0], RString.MSP_Chance);
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.TextFieldPercent(rect_chance[1], ref chance, ref chanceBuf, 0f, 1f);

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_gender[0], R.MoreScenarioParts.String.MSP_Gender);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_gender[1], gender.Translate()))
            {
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<PawnModifierGender>(), ScenPartExtensions.Translate, (g) => () => gender = g);
            }

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_context[0], RString.MSP_Context);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_context[1], context.Translate(), true, false, true))
            {
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<PawnModifierContext>(), ScenPartExtensions.Translate, (c) => () => context = c);
            }

            if (isFaction)
            {
                Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(rect_faction[0], RString.MSP_Faction);
                Text.Anchor = TextAnchor.UpperLeft;

                if (Widgets.ButtonText(rect_faction[1], faction.LabelCap, true, false, true))
                {
                    FloatMenuUtility.MakeMenu(DefDatabase<FactionDef>.AllDefs.Where((d) => !d.isPlayer), (f) => f.LabelCap, (f) => () => faction = f);
                }
            }
        }

        public override PawnGenerationRequest Notify_PawnGenerationRequest(PawnGenerationRequest req)
        {
            if (req.FixedGender.HasValue)
                return req;

            if (!Rand.Chance(chance))
                return req;

            Gender? g = gender == PawnModifierGender.Female ? Gender.Female : Gender.Male;
            bool isPlayerFaction = req.Faction?.IsPlayer ?? false;
            switch (context)
            {
                case PawnModifierContext.All:
                    return req.WithProperty(nameof(PawnGenerationRequest.FixedGender), g);
                case PawnModifierContext.Player:
                    if (isPlayerFaction)
                        return req.WithProperty(nameof(PawnGenerationRequest.FixedGender), g);
                    break;
                case PawnModifierContext.NonPlayer:
                    if (!isPlayerFaction)
                        return req.WithProperty(nameof(PawnGenerationRequest.FixedGender), g);
                    break;
                case PawnModifierContext.Faction:
                    if (faction == req.Faction.def)
                        return req.WithProperty(nameof(PawnGenerationRequest.FixedGender), g);
                    break;
                case PawnModifierContext.PlayerStarter:
                    if (req.Context == PawnGenerationContext.PlayerStarter)
                        return req.WithProperty(nameof(PawnGenerationRequest.FixedGender), g);
                    break;
            }

            return req;
        }


        public override bool CanCoexistWith(ScenPart other)
        {
            // TODO: Fix
            return true;
        }
    }
}