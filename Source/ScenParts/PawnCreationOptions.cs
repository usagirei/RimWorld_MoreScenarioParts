using RimWorld;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    public class PawnCreationOptions : ThingComp
    {
        private PawnGenerationRequest request;
        private bool spawnedOnMapGeneration;

        public PawnGenerationContext Context
        {
            get => request.Context;
        }

        public bool IsStartingPawn
        {
            get => request.Context == PawnGenerationContext.PlayerStarter;
        }

        public PawnGenerationRequest Request
        {
            get => request;
            set => request = value;
        }

        public bool SpawnedOnMapGeneration
        {
            get => spawnedOnMapGeneration;
            set => spawnedOnMapGeneration = value;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref request, nameof(request));
            Scribe_Values.Look(ref spawnedOnMapGeneration, nameof(spawnedOnMapGeneration), false);
        }
    }
}
