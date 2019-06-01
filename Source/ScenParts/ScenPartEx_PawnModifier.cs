using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;



namespace More_Scenario_Parts.ScenParts
{
    public class ScenPartEx_PawnModifier : ScenPartEx
    {
        protected float chance = 1f;
        protected PawnModifierContext context;
        protected PawnModifierGender gender;
        protected FactionDef faction;

        private string chanceBuf;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref chance, nameof(chance), 0f, false);
            Scribe_Values.Look<PawnModifierContext>(ref this.context, nameof(context), PawnModifierContext.All, false);
            Scribe_Defs.Look<FactionDef>(ref faction, nameof(faction));
            Scribe_Values.Look<PawnModifierGender>(ref gender, nameof(gender));

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
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<PawnModifierGender>(), ScenPartExtensions.Translate, (g) => () => gender = g);
            }

            // Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect_context[0], R.String.MSP_Context.CapitalizeFirst());
            // Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(rect_context[1], context.Translate(), true, false, true))
            {
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<PawnModifierContext>(), ScenPartExtensions.Translate, (c) => () => context = c);
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

        public override void Randomize()
        {
            chance = GenMath.RoundedHundredth(Rand.Range(0.05f, 1f));
            context = Utilities.GetEnumValues<PawnModifierContext>().RandomElement();
            faction = faction = DefDatabase<FactionDef>.AllDefs.Where(f => !f.isPlayer).RandomElement();
            gender = Utilities.GetEnumValues<PawnModifierGender>().RandomElement();
        }

        public sealed override void Notify_NewPawnGenerating(Pawn pawn, PawnGenerationContext context)
        {
            if (!this.gender.Includes(pawn.gender))
                return;

            if (!Rand.Chance(chance))
                return;

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = context == PawnGenerationContext.PlayerStarter;

            switch (this.context)
            {
                case PawnModifierContext.All:
                    ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Player:
                    if (isPlayerFaction)
                        ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;
                case PawnModifierContext.NonPlayer:
                    if (!isPlayerFaction)
                        ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerStarter:
                    if(context == PawnGenerationContext.PlayerStarter)
                        ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;
                case PawnModifierContext.PlayerNonStarter:
                    if (context == PawnGenerationContext.NonPlayer && isPlayerFaction)
                        ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Faction:
                    if (pawn.Faction.def == faction)
                        ModifyNewPawn(pawn, pawn.RaceProps.Humanlike);
                    break;
            }
        }

        public sealed override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context, bool redressed)
        {
            if (!this.gender.Includes(pawn.gender))
                return;

            if (!Rand.Chance(chance))
                return;

            bool isPlayerFaction = pawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = context == PawnGenerationContext.PlayerStarter;

            switch (this.context)
            {
                case PawnModifierContext.All:
                    ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Player:
                    if (isPlayerFaction)
                        ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;
                case PawnModifierContext.NonPlayer:
                    if (!isPlayerFaction)
                        ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerStarter:
                    if(context == PawnGenerationContext.PlayerStarter)
                        ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;
                case PawnModifierContext.PlayerNonStarter:
                    if (context == PawnGenerationContext.NonPlayer && isPlayerFaction)
                        ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Faction:
                    if (pawn.Faction.def == faction)
                        ModifyGeneratedPawn(pawn, redressed, pawn.RaceProps.Humanlike);
                    break;
            }
        }

        public sealed override void Notify_PawnDied(Corpse corpse)
        {
            Pawn pawn = corpse.InnerPawn;

            if (!this.gender.Includes(corpse.InnerPawn.gender))
                return;

            if (!Rand.Chance(chance))
                return;

            var opts = corpse.InnerPawn.GetComp<PawnCreationOptions>();

            bool isPlayerFaction = corpse.InnerPawn.Faction?.IsPlayer ?? false;
            bool isStartingPawn = opts.IsStartingPawn;
            switch (this.context)
            {
                case PawnModifierContext.All:
                    ModifyDeadPawn(corpse, corpse.InnerPawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Player:
                    if (isPlayerFaction)
                        ModifyDeadPawn(corpse, corpse.InnerPawn.RaceProps.Humanlike);
                    break;
                case PawnModifierContext.NonPlayer:
                    if (!isPlayerFaction)
                        ModifyDeadPawn(corpse, corpse.InnerPawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.PlayerStarter:
                    if(isStartingPawn)
                        ModifyDeadPawn(corpse, corpse.InnerPawn.RaceProps.Humanlike);
                    break;
                case PawnModifierContext.PlayerNonStarter:
                    if (isPlayerFaction && !isStartingPawn)
                        ModifyDeadPawn(corpse, corpse.InnerPawn.RaceProps.Humanlike);
                    break;

                case PawnModifierContext.Faction:
                    if (corpse.Faction.def == faction)
                        ModifyDeadPawn(corpse, corpse.InnerPawn.RaceProps.Humanlike);
                    break;
                   
            }
        }

        public override void PostMapGenerate(Map map)
        {

        }

        protected virtual void ModifyNewPawn(Pawn p, bool humanLike)
        {

        }

        protected virtual void ModifyGeneratedPawn(Pawn p, bool redressed, bool humanLike)
        {

        }

        protected virtual void ModifyDeadPawn(Corpse c, bool humanLike)
        {

        }
    }
}
