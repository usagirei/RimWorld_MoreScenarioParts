using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using More_Scenario_Parts.ScenParts;
using RimWorld;
using Verse;

namespace More_Scenario_Parts
{
    
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        private static Action<Pawn> discardGeneratedPawn;
        private static IList pawnsBeingGenerated;

        static HarmonyPatches()
        {
            HarmonyInstance instance = HarmonyInstance.Create("rimworld.usagirei.more_scenario_parts");

            instance.Patch(
                original: AccessTools.Method(typeof(PawnGenerator), "TryGenerateNewPawnInternal"),
                prefix: null,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(TryGenerateNewPawnInternal_ScenPartExSupport))
            );

            instance.Patch(
               original: AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) }),
               prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GeneratePawn_ScenPartExSupport)),
               postfix: null
           );

            instance.Patch(
               original: AccessTools.Method(typeof(GameInitData), nameof(GameInitData.PrepForMapGen)),
               prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PrepForMapGen_ScenPartExSupport)),
               postfix: null
           );

            discardGeneratedPawn = (Action<Pawn>) Delegate.CreateDelegate(typeof(Action<Pawn>), AccessTools.Method(typeof(PawnGenerator), "DiscardGeneratedPawn"));
            pawnsBeingGenerated = (IList) AccessTools.Field(typeof(PawnGenerator), "pawnsBeingGenerated").GetValue(null);

            Log.Message("More Scenario Parts Hooks Initialized");
        }



        private static void TryGenerateNewPawnInternal_ScenPartExSupport(ref PawnGenerationRequest request, ref string error, bool ignoreScenarioRequirements, ref Pawn __result)
        {
            if (__result == null)
                return;

            var pgsType = typeof(PawnGenerator).GetNestedType("PawnGenerationStatus", System.Reflection.BindingFlags.NonPublic);

            pawnsBeingGenerated.Add(Activator.CreateInstance(pgsType, new[] { __result, null }));
            try
            {
                if (!ignoreScenarioRequirements && request.Context == PawnGenerationContext.NonPlayer && Find.Scenario != null && !ScenarioUtils.AllowWorldStartingPawn(__result, false, request))
                {
                    discardGeneratedPawn(__result);
                    error = "World Generated pawn doesn't meet scenario requirements.";
                    __result = null;
                }
            }
            finally
            {
                pawnsBeingGenerated.RemoveAt(pawnsBeingGenerated.Count - 1);
            }

            var opts = __result.GetComp<PawnCreationOptions>();
            opts.Request = request;
        }

        private static bool GeneratePawn_ScenPartExSupport(ref PawnGenerationRequest request)
        {
            ScenarioUtils.BeforeGeneratePawn(ref request);
            return true;
        }

        private static bool PrepForMapGen_ScenPartExSupport()
        {
            ScenarioUtils.PrepForMapGen();
            return true;
        }
    }

    static class ScenarioUtils
    {
        public static bool AllowWorldStartingPawn(Pawn p, bool tryingToRedress, PawnGenerationRequest req)
        {
            foreach (var part in Find.Scenario.AllParts)
            {
                if (part is ScenPartEx exp)
                {
                    if (!exp.AllowWorldGeneratedPawn(p, tryingToRedress, req))
                        return false;
                }
            }
            return true;
        }

        public static bool BeforeGeneratePawn(ref PawnGenerationRequest req)
        {
            foreach (var part in Find.Scenario.AllParts)
            {
                if (part is ScenPartEx exp)
                {
                    req = exp.Notify_PawnGenerationRequest(req);
                }
            }
            return true;
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
