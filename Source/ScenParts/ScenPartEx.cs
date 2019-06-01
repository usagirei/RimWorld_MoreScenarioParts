using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class ScenPartEx : ScenPart
    {
        public virtual PawnGenerationRequest Notify_PawnGenerationRequest(PawnGenerationRequest req)
        {
            return req;
        }

        public virtual bool AllowWorldGeneratedPawn(Pawn pawn, bool tryingToRedress, PawnGenerationRequest req)
        {
            return true;
        }
    }
}
