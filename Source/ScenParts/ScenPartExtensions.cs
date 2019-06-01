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
                    return RString.MSP_ThingKind_Weapon;
                case ThingKind.Aparrel:
                    return RString.MSP_ThingKind_Aparrel;
                case ThingKind.Building:
                    return RString.MSP_ThingKind_Building;
                case ThingKind.Items:
                    return RString.MSP_ThingKind_Items;
                default:
                    return RString.MSP_Undefined;
            }
        }

        public static string Translate(this PawnModifierContext c)
        {
            switch (c)
            {
                case PawnModifierContext.All:
                    return RString.MSP_PawnContext_All;
                case PawnModifierContext.PlayerStarter:
                    return RString.MSP_PawnContext_PlayerStarter;
                case PawnModifierContext.NonPlayer:
                    return RString.MSP_PawnContext_NonPlayer;
                case PawnModifierContext.Faction:
                    return RString.MSP_PawnContext_Faction;
                case PawnModifierContext.Player:
                    return RString.MSP_PawnContext_Player;
                default:
                    return RString.MSP_Undefined;
            }
        }

        public static string Translate(this PawnModifierGender c)
        {
            switch (c)
            {
                case PawnModifierGender.All:
                    return RString.MSP_Gender_All;
                case PawnModifierGender.Male:
                    return RString.MSP_Gender_Male;
                case PawnModifierGender.Female:
                    return RString.MSP_Gender_Female;
                default:
                    return RString.MSP_Undefined;
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
