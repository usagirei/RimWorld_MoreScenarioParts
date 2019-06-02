# More Scenario Parts

Adds more Scenario Parts, also changes some of the vanilla ones to allow better control

## Filtering

Modifier filtering allows for finer-grained control over:

#### Context:

  * All Pawns
  * Player Starting Pawns
  * Player Non-Starting Pawns
  * Non-Player Pawns
  * Player Faction Pawns (Starting and new joins)
  * Non-Player Faction

#### Gender

  * All Pawns
  * Only Male Pawns
  * Only Female Pawns

#### Faction

  * Pick a specific faction when using the `Non-Player Faction` Context

#### Chance

* Chance of the modifier being applied on a pawn upon generation

## Available Scenario Parts

#### New Scenario Parts

* Pawn Gender
  * *Ensure pawns are of a specific gender, where applicable*
* Pawn Hair Color
  * *Assigns random color from allowed red, green and blue ranges*
* Pawn Skin Color
  * *Assigns random skin tone from allowed range*
* Inventory
  * *Start with equipped gear / item stack in pawn inventory*

#### Vanilla Reworks

Modifies some of vanilla scenario parts with the extended filtering options

* Allowed age range
  * *Just like vanilla filter, may spew a few errors if fail to generate a pawn after so many tries*
* Forced health condition
* Forced trait
* Naked
* Need level
* On pawn death explosion
  * *Also allows for more damage types to be chosen from*

## Compatibility

Tested with RimWorld 1.0

Requires a new save to make use of the new scenario filters

Incompatibilities unknown as of now, but should be compatible with any mods.
* XML Patches:
  * Adds a *ThingComp* def to *BasePawn*
  * Replaces the default scenario classes with the modded ones
* Harmony Patches: 
  * PawnGenerator: `TryGenerateNewPawnInternal`, `GeneratePawn`
  * GameInitData: `PrepareForMapGen`