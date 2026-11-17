# 04: Move the remaining prompts onto Prompt

**What to build:** The main menu, race, occupation, the three stat rolls, the menu after character creation and the combat round all ask through Prompt. Their answers come from choice maps: the existing race and occupation maps, plus new main menu, next action, roll and combat choice maps in all four languages, holding today's words. The main menu uses the same-line flag. Character creation and combat receive the game's Prompt. The name prompt and the language menu keep their own code. Nothing the player sees changes, and ticket 01's combat behaviour is kept. See `.scratch/prompt-module/spec.md`.

**Blocked by:** 01 (Invalid answer at the combat prompt asks again), 03 (Prompt module, used by the four Inventory prompts)

**Status:** ready-for-agent

- [ ] Main menu (`1`/`2`), next action (`1`/`2`), roll (`roll`) and combat (`attack`/`a`/`retreat`/`r`) choice maps added to all four languages; the coverage test covers them.
- [ ] Race, occupation, the three rolls and the next-action menu use Prompt; the next-action caller loops on "list stats". The name prompt is unchanged.
- [ ] The main menu uses Prompt with the same-line flag and still shows "Enter choice:" without the Inventory hint.
- [ ] The combat round uses Prompt; an invalid answer (including `i`) still asks again with no extra attack round.
- [ ] The text module's race and occupation validate/normalise methods are deleted; their test cases (including translated race and occupation words) move to the Prompt tests.
- [ ] Call sites use named arguments.
- [ ] Whole-game, character creation, combat and text-module tests pass with only setup changes.
