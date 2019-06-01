using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    class PawnCreationOptions : ThingComp
    {
        private PawnGenerationRequest request;
        private bool spawnedOnMapGeneration;
         

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref request, nameof(request));
            Scribe_Values.Look(ref spawnedOnMapGeneration, nameof(spawnedOnMapGeneration), false);
        }

        public PawnGenerationContext Context
        {
            get => request.Context;
        }

        public bool IsStartingPawn
        {
            get => request.Context == PawnGenerationContext.PlayerStarter;
        }

        public bool SpawnedOnMapGeneration
        {
            get => spawnedOnMapGeneration;
            set => spawnedOnMapGeneration = value;
        }

        public PawnGenerationRequest Request
        {
            get => request;
            set => request = value;
        }
    }
}
