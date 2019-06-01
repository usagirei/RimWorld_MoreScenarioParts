using RimWorld;
using Verse;
using RString = R.MoreScenarioParts.String;

namespace More_Scenario_Parts.ScenParts
{
    public static class ScenPartExtensions
    {
        public static string Translate(this ThingKind thing)
        {
            switch (thing)
            {
                case ThingKind.Weapon:
                    return RString.MSP_ThingKind_Weapon.CapitalizeFirst();
                case ThingKind.Aparrel:
                    return RString.MSP_ThingKind_Aparrel.CapitalizeFirst();
                case ThingKind.Building:
                    return RString.MSP_ThingKind_Building.CapitalizeFirst();
                case ThingKind.Items:
                    return RString.MSP_ThingKind_Items.CapitalizeFirst();
                default:
                    return RString.MSP_Undefined.CapitalizeFirst();
            }
        }

        public static string Translate(this PawnModifierContext c)
        {
            switch (c)
            {
                case PawnModifierContext.All:
                    return RString.MSP_PawnContext_All.CapitalizeFirst();
                case PawnModifierContext.PlayerStarter:
                    return RString.MSP_PawnContext_PlayerStarter.CapitalizeFirst();
                case PawnModifierContext.NonPlayer:
                    return RString.MSP_PawnContext_NonPlayer.CapitalizeFirst();
                case PawnModifierContext.Faction:
                    return RString.MSP_PawnContext_Faction.CapitalizeFirst();
                case PawnModifierContext.Player:
                    return RString.MSP_PawnContext_Player.CapitalizeFirst();
                case PawnModifierContext.PlayerNonStarter:
                    return RString.MSP_PawnContext_PlayerNonStarter.CapitalizeFirst();
                default:
                    return RString.MSP_Undefined.CapitalizeFirst();
            }
        }

        public static string Translate(this PawnModifierGender c)
        {
            switch (c)
            {
                case PawnModifierGender.All:
                    return RString.MSP_Gender_All.CapitalizeFirst();
                case PawnModifierGender.Male:
                    return RString.MSP_Gender_Male.CapitalizeFirst();
                case PawnModifierGender.Female:
                    return RString.MSP_Gender_Female.CapitalizeFirst();
                default:
                    return RString.MSP_Undefined.CapitalizeFirst();
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
    }
}
