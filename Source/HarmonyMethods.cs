using More_Scenario_Parts.ScenParts;
using Verse;

namespace More_Scenario_Parts
{
    static class HarmonyMethods
    {
        public static bool AllowWorldStartingPawn(Pawn p, bool tryingToRedress, PawnGenerationRequest req)
        {
            foreach (var part in Find.Scenario.AllParts)
            {
                if (part is ScenPartEx partEx && !partEx.AllowWorldGeneratedPawn(p, tryingToRedress, req))
                {
                    return false;
                }
            }
            return true;
        }

        public static void BeforeGeneratePawn(ref PawnGenerationRequest req)
        {
            foreach (var part in Find.Scenario.AllParts)
            {
                if (part is ScenPartEx exp)
                {
                    req = exp.Notify_PawnGenerationRequest(req);
                }
            }
        }

        internal static void PrepForMapGen()
        {
            var initData = Find.GameInitData;
            var pawns = initData.startingAndOptionalPawns;
            var startWith = initData.startingPawnCount;

            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn p = pawns[i];
                var opts = p.GetComp<PawnCreationOptions>();
                opts.SpawnedOnMapGeneration = i < startWith;
            }
        }
    }
}
