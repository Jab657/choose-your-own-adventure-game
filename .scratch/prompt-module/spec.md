# Spec: Prompt module

Status: ready-for-agent

## Problem Statement

The game asks the player a question in 11 places: the main menu, race, occupation, the three stat rolls, the menu after character creation, the path, the Merchant, the Shrine, the dragon encounter and each combat round. Each place has its own hand-written "ask until the answer is valid" loop, and the copies no longer agree with each other.

For the player, this shows up as one real bug. At the combat prompt, an answer that isn't attack or retreat prints "invalid" and then starts another attack round anyway. A typo, or typing `i` out of habit from the other prompts, makes the player attack.

For the developer, every change to how questions work has to be repeated by hand:

- The Inventory feature pasted the same Inventory check and hint into four prompts. Any future change to checking the Inventory means four edits.
- Some prompts take their accepted answers from the per-language data; others compare against English words written into the game logic.
- The text module has grown one special-purpose method per prompt and one copy-pasted loader per map, so each new prompt widens it.
- The game still carries three validation methods that nothing calls. They repeat the path and combat words and would silently disagree once those words move into the language data.

## Solution

One **Prompt** module (see the glossary) owns asking a question:

- printing it,
- reading and tidying the answer,
- looking it up in the current language's accepted answers,
- offering the Inventory where allowed,
- printing the invalid message and asking again,
- stopping cleanly when input ends.

Each place that asks a question only says which question, which answers are accepted, which invalid message to show, and whether the Inventory is offered.

For the player, everything looks and behaves exactly as it does today, with one exception. An invalid answer at the combat prompt now asks the question again instead of starting another attack round, the same as every other prompt.

## User Stories

1. As a player, I want an invalid answer at the combat prompt to ask me again, so that a typo doesn't make me attack.
2. As a player, I want typing `i` at the combat prompt to be an invalid answer that asks me again, so that the Inventory doesn't interrupt the fight and I don't attack by accident.
3. As a player, I want attack and retreat at the combat prompt to work as they do today, so that fighting and fleeing are unchanged.
4. As a player, I want the main menu to look and behave exactly as it does today, including typing my answer after "Enter choice:" on the same line.
5. As a player, I want the race, occupation and roll prompts to look and behave exactly as they do today.
6. As a player, I want the menu after character creation to still show my stats on "1" and ask again, and to proceed on "2".
7. As a player, I want the path prompt to keep its own invalid message, so that the game still tells me which paths exist.
8. As a player, I want the race and occupation prompts to keep their own invalid messages.
9. As a player, I want checking the Inventory at the path, Merchant, Shrine and dragon prompts to keep working exactly as before: free, changing nothing, and showing the same prompt again.
10. As a player, I want the Inventory hint to appear at those four prompts and nowhere else, as it does today.
11. As a player, I want answers to keep working regardless of capitals or surrounding spaces at every prompt.
12. As a player, I want "not enough Gold" at the Merchant or the Shrine to keep showing the prompt again, so that I can choose something else.
13. As a player in Spanish, French or Italian, I want the words I can type today (`north`, `attack`, `roll`, `1`, `2` and my own language's race, occupation, Merchant, Shrine and Inventory words) to keep working.
14. As a player whose input ends at any prompt, I want the game to stop cleanly, so that it never loops forever.
15. As a player, I want never to see raw placeholder text or a crash at language selection, so that the game feels finished in every language.
16. As a developer, I want "ask until valid" written once, so that a fix to it is fixed at every prompt.
17. As a developer, I want a new prompt to be a single call, so that I can't forget the retry, the Inventory hint or end-of-input handling.
18. As a developer, I want to say per prompt whether the Inventory is offered, so that adding or removing it is a one-word change.
19. As a developer, I want the Prompt module to know nothing about Gold, combat or any other game rule, so that game rules stay with the encounter that owns them.
20. As a developer, I want every prompt's accepted answers to live in the language data, so that translating them later is a data change only.
21. As a developer, I want the text module to load every map the same way, so that adding a map doesn't mean adding another loader.
22. As a developer, I want one choice lookup instead of one method per prompt, so that adding a prompt doesn't widen the text module.
23. As a developer, I want the coverage test to fail if any language is missing a choice map, or a map's answers mean something different from English, so that a missing map can't crash the game at language selection.
24. As a developer, I want the "ask until valid" rules tested once, directly through the Prompt module, so that I don't need a whole-game run to check them.
25. As a developer, I want the existing whole-game tests to keep passing with only setup changes, so that I have evidence nothing visible changed.
26. As a developer, I want the combat fix covered by a test that failed before the fix, so that the bug can't come back.
27. As a developer, I want the three unused game validation methods and their tests gone, so that there is one source of truth for accepted answers.
28. As a developer, I want call sites to name each text key they pass, so that swapping the question, the map and the invalid message is easy to spot.

## Implementation Decisions

- **Domain term**: **Prompt** is defined in the glossary: a question the game asks the player, repeated until they give one of its accepted answers, some of which let the player check the Inventory.
- **Prompt module**: a new module, created once per game from the text module after the language is chosen. It has one operation, "ask". It takes:
  - the question's message key,
  - the name of the choice map holding the accepted answers,
  - the invalid message key,
  - optionally, the player whose Inventory is offered (absent means the Inventory isn't offered),
  - optionally, a flag asking for the answer on the same line.
- **What "ask" prints**:
  1. the question, on its own line;
  2. if the Inventory is offered, the Inventory hint on its own line;
  3. if the Inventory is offered or the same-line flag is set, "Enter choice:" without a line break.
- **How "ask" reads and answers**:
  - It reads through the existing shared console input and trims and lowercases the answer.
  - If the Inventory is offered and the answer is the Inventory command, it shows that player's Inventory and asks the whole question again.
  - Otherwise, if the answer is in the choice map, it returns that answer's canonical value.
  - Otherwise, it prints the invalid message and asks the whole question again.
- **What "ask" guarantees**:
  - It never returns an invalid or empty answer.
  - The only failure is the existing end-of-input signal, which the game's start-up still catches.
  - When the Inventory isn't offered, the Inventory command is treated like any other answer, so it is invalid at the combat prompt.
- **Return value**: the canonical English value as a string (`north`, `armor`, `Elf`, `offer`). Callers that use an enum convert it themselves; the Merchant converts to its item type and the Shrine to its choice type.
- **Game rules stay with the callers.** When a valid answer leads to asking again, the caller loops around "ask". This covers "not enough Gold" at the Merchant and Shrine, "list stats" at the menu after character creation, and "attack" in a combat round.
- **Passing the Prompt**: the game passes its Prompt, alongside the text module, to character creation, the Merchant visit, the Shrine visit, the dragon encounter and combat. Their interfaces change accordingly.
- **Call sites** use named arguments for the three text keys.
- **Prompt wiring**, matching today's layout exactly:

  | Prompt | Question key | Choice map | Invalid key | Inventory | Same line |
  |---|---|---|---|---|---|
  | Main menu | `menu` | `mainMenuChoiceMap` | `invalid` | no | yes |
  | Race | `race_prompt` | `raceMap` | `race_invalid` | no | no |
  | Occupation | `occupation_prompt` | `occupationMap` | `occupation_invalid` | no | no |
  | Roll ×3 | `roll_strength` / `roll_agility` / `roll_health` | `rollChoiceMap` | `roll_invalid` | no | no |
  | Next action | `next_action` | `nextActionChoiceMap` | `invalid` | no | no |
  | Path | `path_prompt_full` | `pathChoiceMap` | `path_invalid` | yes | (implied) |
  | Merchant | `merchant_prompt` | `merchantChoiceMap` | `invalid` | yes | (implied) |
  | Shrine | `shrine_prompt` | `shrineChoiceMap` | `invalid` | yes | (implied) |
  | Dragon | `dragon_encounter_prompt` | `dragonChoiceMap` | `invalid` | yes | (implied) |
  | Combat | `attack_prompt` | `combatChoiceMap` | `invalid` | no | no |

  The roll prompts ignore the returned value.
- **Not covered by Prompt**:
  - The name prompt is free text, not a choice, so it keeps its own loop.
  - The language menu keeps its own code. It runs before the text module is loaded, and any unrecognised answer picks English.
- **Text module**:
  - It loads every map section of the chosen language with one loader instead of one copy per map, display maps included.
  - Every map is string to string. The Shrine choice map is no longer turned into an enum inside the text module.
  - It gains one choice lookup, which only the Prompt module uses.
  - These are deleted, not kept as wrappers: `IsValidRace`, `NormalizeRace`, `IsValidOccupation`, `NormalizeOccupation`, `NormalizeMerchantChoice`, `NormalizeShrineChoice` and `IsInventoryCommand`.
  - The message lookup and the display translations are unchanged.
- **New choice maps** go into all four language sections. Each holds exactly the answers accepted today, mapped to English canonical values, with the same words in every language:
  - main menu: `1`, `2`
  - next action: `1`, `2`
  - roll: `roll`
  - path: `north`, `n`, `south`, `s`, `exit`, `e`
  - dragon: `attack`, `a`, `retreat`, `r`
  - combat: `attack`, `a`, `retreat`, `r`

  This follows ADR-0001: every language complete, no accented characters.
- **Combat fix**: an invalid answer at the combat prompt prints the invalid message and asks again, with no extra attack round. It is fixed on the current code before the refactor, so the refactor itself changes nothing the player sees.
- **Game clean-up**: the three unused validation methods (menu, path and combat choice) are deleted.
- **Delivery order**:
  1. the combat fix;
  2. the Prompt module plus the four prompts that offer the Inventory;
  3. the remaining prompts and the deletion of the old text-module methods;
  4. the deletion of the unused game validation methods.

## Testing Decisions

- **What makes a good test**: it asserts on external behaviour through a module's interface: printed text and its order, returned values, and player state. It should keep passing if internals change. Tests never reach past an interface.
- **Seams, from highest to lowest:**
  1. **The whole game with scripted console input** (existing, the regression seam). The Inventory, end-of-input, North path and game-flow tests and the Merchant and Shrine visit tests keep their assertions. Only their setup changes where interfaces change. They are the evidence that nothing visible changed.
  2. **The Prompt module** (new, the main seam). Driven by swapping the console the way the existing suites do. It covers:
     - a valid answer returns its canonical value;
     - mixed case and surrounding spaces still match;
     - an invalid answer prints the given invalid message, then the whole question again, then returns the next valid answer;
     - with the Inventory offered: the hint and "Enter choice:" are printed, the Inventory command shows the Inventory and asks again without the invalid message, and the player is unchanged;
     - without the Inventory offered: the Inventory command is invalid and no hint is printed;
     - the same-line flag prints "Enter choice:" without the hint;
     - input ending raises the end-of-input signal;
     - translated answers, such as a Spanish race word or an Italian Inventory word. These replace today's direct tests of the per-prompt text-module methods, so the choice lookup is tested only through Prompt.
  3. **Combat, built directly** (existing). Used only for the bug fix, because a whole-game run can't reach a combat round reliably while the player's die can't be scripted. Give the player and the dragon enough health that nobody dies, script an invalid answer then retreat, and assert exactly one attack round happened before the retreat. Also check that `i` gets the invalid message. Write this test first, so it fails before the fix.
  4. **The text module** (existing). Used only for the data-coverage check. Extend the existing key-coverage test so that every language has every map English has, and each choice map's set of canonical values equals English's.
- **Removed tests**:
  - the game tests for the three deleted validation methods;
  - the direct tests of the deleted per-prompt text-module methods, whose cases move up to the Prompt seam.
- **Prior art**:
  - the console-swapping helpers in the Merchant and Shrine tests;
  - the whole-game runs in the Inventory and end-of-input tests;
  - the key-coverage test in the text-module tests;
  - the combat tests that build combat directly.

## Out of Scope

- Injecting a console adapter in place of the static console (architecture candidate 2). Prompt is the module that will receive it later.
- Translating the accepted answers at the path, dragon, combat and roll prompts, or the numbered menus.
- Making the prompts uniform: one invalid message, "Enter choice:" everywhere, or the Inventory at more prompts.
- Shrinking the per-prompt rows duplicated across the Inventory and end-of-input tests.
- Typed enum return values from Prompt, or an object bundling each prompt's three keys.
- Injecting dice into the game and the player, or splitting character creation out of the player.

## Further Notes

- Use the glossary terms (Prompt, Inventory, Merchant, Shrine, Gold) in code, messages and tests.
- The combat fix is the only player-visible change. Any other difference in output is a regression.
- The same-line flag exists only for the main menu. If a later change gives every menu-style prompt "Enter choice:", the flag can go.
