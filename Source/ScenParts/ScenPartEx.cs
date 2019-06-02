using RimWorld;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class ScenPartEx : ScenPart
    {
        public virtual bool AllowWorldGeneratedPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return true;
        }

        public virtual PawnGenerationRequest Notify_PawnGenerationRequest(PawnGenerationRequest req)
        {
            return req;
        }
    }
}
