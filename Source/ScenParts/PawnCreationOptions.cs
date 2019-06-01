using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace More_Scenario_Parts.ScenParts
{
    class PawnCreationOptions : ThingComp
    {
        private PawnCreationOptionProps Props => (PawnCreationOptionProps) props;

        public bool IsStartingPawn
        {
            get => Props.isStartingPawn;
            set => Props.isStartingPawn = true;
        }
    }

    class PawnCreationOptionProps : CompProperties
    {
        public bool isStartingPawn;
    }
}
