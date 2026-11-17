# 03: Move the remaining prompts onto Prompt

**What to build:** Move the main menu, race, occupation, the three stat rolls, the next-action menu and the combat round onto the Prompt module from ticket 02. Their answers come from choice maps: the existing `raceMap` and `occupationMap`, plus new `mainMenuChoiceMap`, `nextActionChoiceMap`, `rollChoiceMap` and `combatChoiceMap` in every language, holding today's words. The main menu uses `answerOnSameLine`. The name prompt and the language menu keep their own code. Nothing the player sees changes. See `.scratch/prompt-module/spec.md`.

**Blocked by:** 02 (Prompt module, used by the Inventory prompts)

**Status:** ready-for-agent

- [ ] `mainMenuChoiceMap` (`1`/`2`), `nextActionChoiceMap` (`1`/`2`), `rollChoiceMap` (`roll`) and `combatChoiceMap` (`attack`/`a`/`retreat`/`r`) added to all four languages.
- [ ] `Player.CreateCharacter` takes the Prompt; race, occupation, the three rolls and the next-action menu use it. `PromptForRace`, `PromptForOccupation` and `PromptRoll` loops are gone; the next-action caller loops on "1. List stats". `PromptForName` is unchanged.
- [ ] The main menu uses Prompt with `answerOnSameLine: true`.
- [ ] `Combat` takes the Prompt for its attack/retreat prompt; ticket 01's behaviour is kept.
- [ ] `IsValidRace`, `NormalizeRace`, `IsValidOccupation` and `NormalizeOccupation` are deleted. Their test cases (including translated race and occupation words) move up to `PromptTests`.
- [ ] Call sites use named arguments.
- [ ] Existing whole-game, Player, Combat and Messages tests pass with only setup changes; the extended key-coverage test covers the new maps.
