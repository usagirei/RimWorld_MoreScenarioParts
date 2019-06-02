using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class ScenPartEx_PawnModifier : ScenPartEx
    {
        protected float chance = 1f;
        protected PawnModifierContext context;
        protected FactionDef faction;
        protected PawnModifierGender gender;
        private string chanceBuf;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref chance, nameof(chance), 0f, false);
            Scribe_Values.Look(ref context, nameof(context), PawnModifierContext.All, false);
            Scribe_Defs.Look(ref faction, nameof(faction));
            Scribe_Values.Look(ref gender, nameof(gender));
        }

        public sealed override void Notify_NewPawnGenerating(Pawn pawn, PawnGenerationContext coreContext)
        {
            if (!gender.Includes(pawn.gender))
            {
                return;
            }

            if (!Rand.Chance(chance))
            {
                return;
            }

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = coreContext == PawnGenerationContext.PlayerStarter;

            switch (context)
            {
                case PawnModifierContext.All:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Player when isPlayerFaction:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.NonPlayer when !isPlayerFaction:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerStarter when isStartingPawn:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerNonStarter when !isStartingPawn && isPlayerFaction:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Faction when pawn.Faction.def == faction:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;
            }
        }

        public sealed override void Notify_PawnDied(Corpse corpse)
        {
            Pawn pawn = corpse.InnerPawn;

            if (!gender.Includes(pawn.gender))
            {
                return;
            }

            if (!Rand.Chance(chance))
            {
                return;
            }

            var opts = pawn.GetComp<PawnCreationOptions>();

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = opts.IsStartingPawn;
            switch (context)
            {
                case PawnModifierContext.All:
                    ModifyDeadPawn(corpse, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Player when isPlayerFaction:
                    ModifyDeadPawn(corpse, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.NonPlayer when !isPlayerFaction:
                    ModifyDeadPawn(corpse, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerStarter when isStartingPawn:
                    ModifyDeadPawn(corpse, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerNonStarter when isPlayerFaction && !isStartingPawn:
                    ModifyDeadPawn(corpse, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Faction when corpse.Faction.def == faction:
                    ModifyDeadPawn(corpse, pawn.RaceProps.Humanlike);
                    break;
            }
        }

        public sealed override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext coreContext, bool redressed)
        {
            if (!gender.Includes(pawn.gender))
            {
                return;
            }

            if (!Rand.Chance(chance))
            {
                return;
            }

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = coreContext == PawnGenerationContext.PlayerStarter;

            switch (context)
            {
                case PawnModifierContext.All:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Player when isPlayerFaction:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.NonPlayer when !isPlayerFaction:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerStarter when isStartingPawn:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerNonStarter when !isStartingPawn && isPlayerFaction:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Faction when pawn.Faction.def == faction:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;
            }
        }

        public override void PostMapGenerate(Map map)
        {
        }

        public override void Randomize()
        {
            chance = GenMath.RoundedHundredth(Rand.Range(0.05f, 1f));
            context = Extensions.GetEnumValues<PawnModifierContext>().RandomElement();
            faction = faction = DefDatabase<FactionDef>.AllDefs.Where(f => !f.isPlayer).RandomElement();
            gender = Extensions.GetEnumValues<PawnModifierGender>().RandomElement();
        }

        protected void DoContextEditInterface(Rect rect)
        {
            bool isFaction = context == PawnModifierContext.Faction;

            Rect[] rows = rect.SplitRows(1, 1, 1, 1);

            Rect[] rect_chance = rows[0].SplitCols(1, 2);
            Rect[] rect_gender = rows[1].SplitCols(1, 2);
            Rect[] rect_context = rows[2].SplitCols(1, 2);
            Rect[] rect_faction = rows[3].SplitCols(1, 2);

            // Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_chance[0], R.String.MSP_Chance.CapitalizeFirst());
            // Text.Anchor = TextAnchor.UpperLeft;
            Widgets.TextFieldPercent(rect_chance[1], ref chance, ref chanceBuf, 0f, 1f);

            // Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_gender[0], R.String.MSP_Gender.CapitalizeFirst());
            // Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_gender[1], gender.Translate()))
            {
                FloatMenuUtility.MakeMenu(Extensions.GetEnumValues<PawnModifierGender>(), Extensions.Translate, (g) => () => gender = g);
            }

            // Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_context[0], R.String.MSP_Context.CapitalizeFirst());
            // Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_context[1], context.Translate(), true, false, true))
            {
                FloatMenuUtility.MakeMenu(Extensions.GetEnumValues<PawnModifierContext>(), Extensions.Translate, (c) => () => context = c);
            }

            if (isFaction)
            {
                // Text.Anchor = TextAnchor.MiddleRight;
                Widgets.Label(rect_faction[0], R.String.MSP_Faction.CapitalizeFirst());
                // Text.Anchor = TextAnchor.UpperLeft;

                if (Widgets.ButtonText(rect_faction[1], faction.LabelCap, true, false, true))
                {
                    FloatMenuUtility.MakeMenu(DefDatabase<FactionDef>.AllDefs.Where((d) => !d.isPlayer), (f) => f.LabelCap, (f) => () => faction = f);
                }
            }
        }

        protected virtual void ModifyDeadPawn(Corpse c, bool humanLike)
        {
        }

        protected virtual void ModifyGeneratedPawn(Pawn p, bool redressed, bool humanLike)
        {
        }

        protected virtual void ModifyNewPawn(Pawn p, bool humanLike)
        {
        }
    }
}
