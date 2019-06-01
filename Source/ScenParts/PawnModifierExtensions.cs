using RimWorld;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public static class PawnModifierExtensions
    {
        public static string Translate(this PawnModifierContext c)
        {
            switch (c)
            {
                case PawnModifierContext.All:
                    return "MSP_PawnContext_All".Translate();
                case PawnModifierContext.PlayerStarter:
                    return "MSP_PawnContext_PlayerStarter".Translate();
                case PawnModifierContext.NonPlayer:
                    return "MSP_PawnContext_NonPlayer".Translate();
                case PawnModifierContext.Faction:
                    return "MSP_PawnContext_Faction".Translate();
                case PawnModifierContext.Player:
                    return "MSP_PawnContext_Player".Translate();
                default:
                    return "MSP_Undefined".Translate();
            }
        }

        public static string Translate(this PawnModifierGender c)
        {
            switch (c)
            {
                case PawnModifierGender.All:
                    return "MSP_Gender_All".Translate();
                case PawnModifierGender.Male:
                    return "MSP_Gender_Male".Translate();
                case PawnModifierGender.Female:
                    return "MSP_Gender_Female".Translate();
                default:
                    return "MSP_Undefined".Translate();
            }
        }

        public static bool Includes(this PawnModifierGender g, Gender other)
        {
            if (g == PawnModifierGender.All)
                return true;

            if (other == Gender.None)
                return true;

            if (g == PawnModifierGender.Female && other == Gender.Female)
                return true;

            if (g == PawnModifierGender.Male && other == Gender.Male)
                return true;

            return false;
        }

        public static bool Includes(this PawnModifierGender g, PawnModifierGender other)
        {
            if (g == PawnModifierGender.All)
                return true;

            return g == other;
        }

    }
}
