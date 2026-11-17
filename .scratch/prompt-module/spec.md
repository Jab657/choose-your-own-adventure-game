# Spec: Prompt module

Status: ready-for-agent

## Problem Statement

The game asks the player a question in 11 places, and each one has its own hand-written "ask until valid" loop: the main menu, race, occupation, the three stat rolls, the next-action menu after character creation, the path, the Merchant, the Shrine, the dragon encounter and the combat round. Every loop prints a message, reads a line, normalises it, matches it against the accepted answers, and either returns or prints an invalid message and asks again.

The copies have drifted:

- The Inventory feature pasted the same Inventory check and hint into four of them (path, Merchant, Shrine, dragon). Every future change to how checking the Inventory works means editing four places.
- Some loops trim and lowercase the input before matching, some pass it raw and rely on `Messages`.
- Race, occupation, Merchant and Shrine answers come from per-language maps in `language_data.json`; path, dragon, combat, roll and the two numbered menus compare against English words written in C#.
- `Messages` has grown one bespoke method per prompt (`IsValidRace`/`NormalizeRace`, `IsValidOccupation`/`NormalizeOccupation`, `NormalizeMerchantChoice` returning a string, `NormalizeShrineChoice` returning an enum, `IsInventoryCommand`) and loads each map with its own copy-pasted loop.
- **Bug:** at the combat prompt, an invalid answer prints "invalid" and then falls through to another attack round, so typing anything other than attack or retreat attacks.

`Game` also still has three public validation methods (`IsValidMenuChoice`, `IsValidPathChoice`, `IsValidCombatChoice`) that the game never calls. They duplicate the path and combat words and would silently disagree with the maps once those words move into `language_data.json`.

## Solution

One **Prompt** module (see the glossary) owns asking a question: printing it, reading and normalising the answer, looking it up in the current language's choice map, offering the Inventory where allowed, printing the invalid message and asking again, and stopping when input ends. Each caller only says which question to ask, which answers are accepted, which invalid message to use, and whether the Inventory is offered.

This is a pure refactor: everything the player sees stays exactly the same, with one exception. An invalid answer at the combat prompt now asks the question again instead of starting another attack round, like every other prompt.

## User Stories

1. As a player, I want an invalid answer at the combat prompt to ask me again, so that a typo doesn't make me attack.
2. As a player, I want `i` at the combat prompt to still be an invalid answer, so that the Inventory doesn't interrupt the fight.
3. As a player, I want every prompt to look and behave exactly as it does today, so that the refactor changes nothing I can see.
4. As a player, I want checking the Inventory at the path, Merchant, Shrine and dragon prompts to keep working exactly as before.
5. As a player whose input ends at any prompt, I want the game to stop cleanly, so that it doesn't loop forever.
6. As a player in Spanish, French or Italian, I want the words I can type today (`north`, `attack`, `roll`, `1`…) to still work, so that nothing breaks for me.
7. As a developer, I want the "ask until valid" rules tested once, directly, so that I don't need a whole-game run to check them.
8. As a developer, I want a new prompt to be one call, so that I can't forget the retry, the Inventory hint or end-of-input handling.
9. As a developer, I want every prompt's accepted answers to live in `language_data.json`, so that localising them later is a data change only.
10. As a developer, I want the key-coverage test to fail if any language is missing a choice map, or a map's canonical values differ from English, so that a missing map can't crash the game at language selection.
11. As a developer, I want `Messages` to expose one choice lookup instead of one method per prompt, so that adding a prompt doesn't widen its interface.
12. As a developer, I want the unused `Game.IsValid*` methods gone, so that there's only one source of truth for accepted answers.

## Implementation Decisions

- **Prompt interface**:
  ```csharp
  public class Prompt
  {
      public Prompt(Messages messages);
      public string Ask(string promptKey, string choiceMapName, string invalidKey,
                        Player? inventoryOf = null, bool answerOnSameLine = false);
  }
  ```
  - Prints the `promptKey` message (WriteLine). If `inventoryOf` is given, prints `inventory_hint` (WriteLine). If `inventoryOf` is given or `answerOnSameLine` is true, prints `enter_choice` (Write).
  - Reads a line through the existing static `ConsoleInput.ReadLine()`, trims it and lowercases it.
  - If `inventoryOf` is given and the answer is the Inventory command, shows that player's Inventory and asks the whole question again. When the Inventory isn't offered, the Inventory command is just looked up in the map like any other answer (so it's invalid at the combat prompt).
  - If the answer is a key of the named choice map, returns its canonical value. Otherwise prints the `invalidKey` message and asks the whole question again.
  - Never returns null or an invalid answer. The only failure is `EndOfInputException` when input ends, unchanged; `Game.StartGame` still catches it.
- **Callers loop on domain outcomes.** Prompt knows nothing about Gold or any other game rule. When a valid answer leads to "ask again" (not enough Gold at the Merchant or Shrine, "1. List stats" at the next-action menu, "attack" in a combat round), the caller loops around `Ask`.
- **Return type** is the canonical string. Callers that want an enum convert it themselves (Merchant → `MerchantItem`, Shrine → `ShrineChoice`).
- **One Prompt per game.** `Game` creates it from `Messages` after language selection and passes it, alongside `Messages`, to character creation, the Merchant and Shrine visits, the dragon encounter and Combat. Their signatures change accordingly.
- **Call sites use named arguments** (`promptKey:`, `choiceMapName:`, `invalidKey:`) to guard against swapping the three strings.
- **Prompts covered and their wiring** (layout unchanged from today):

  | Prompt | promptKey | choiceMapName | invalidKey | Inventory | answerOnSameLine |
  |---|---|---|---|---|---|
  | Main menu | `menu` | `mainMenuChoiceMap` | `invalid` | no | yes |
  | Race | `race_prompt` | `raceMap` | `race_invalid` | no | no |
  | Occupation | `occupation_prompt` | `occupationMap` | `occupation_invalid` | no | no |
  | Roll ×3 | `roll_strength` / `roll_agility` / `roll_health` | `rollChoiceMap` | `roll_invalid` | no | no |
  | Next action | `next_action` | `nextActionChoiceMap` | `invalid` | no | no |
  | Path | `path_prompt_full` | `pathChoiceMap` | `path_invalid` | yes | — |
  | Merchant | `merchant_prompt` | `merchantChoiceMap` | `invalid` | yes | — |
  | Shrine | `shrine_prompt` | `shrineChoiceMap` | `invalid` | yes | — |
  | Dragon | `dragon_encounter_prompt` | `dragonChoiceMap` | `invalid` | yes | — |
  | Combat | `attack_prompt` | `combatChoiceMap` | `invalid` | no | no |

  The roll prompts' returned value (`"roll"`) is ignored.
- **Not covered**: the name prompt (free text, not a choice) keeps its own loop. The language menu keeps its own code: it runs before `Messages` is loaded, and any unrecognised answer picks English.
- **Messages**:
  - `ReadDictionary` loads every `*Map` section of the language in one loop instead of one copy per map (display maps included). Each map is string → string; `shrineChoiceMap` stops being parsed into `ShrineChoice` inside `Messages`.
  - One choice lookup replaces `IsValidRace`, `NormalizeRace`, `IsValidOccupation`, `NormalizeOccupation`, `NormalizeMerchantChoice`, `NormalizeShrineChoice` and `IsInventoryCommand`. Those methods are deleted, not kept as wrappers. The display lookups (`Translate*ForDisplay`) and `GetMessage` are unchanged.
- **New choice maps** in all four language sections, holding exactly the words accepted today, mapped to English canonical values:
  - `mainMenuChoiceMap`: `1`, `2`
  - `nextActionChoiceMap`: `1`, `2`
  - `rollChoiceMap`: `roll`
  - `pathChoiceMap`: `north`, `n`, `south`, `s`, `exit`, `e`
  - `dragonChoiceMap`: `attack`, `a`, `retreat`, `r`
  - `combatChoiceMap`: `attack`, `a`, `retreat`, `r`

  Translating these words is out of scope; per ADR-0001's follow-ups, it becomes a data-only change later.
- **Combat fix**: an invalid answer at the combat prompt prints `invalid` and asks again, without another attack round. Retreat and attack behave as today.
- **Game clean-up**: `IsValidMenuChoice`, `IsValidPathChoice` and `IsValidCombatChoice` are deleted.

## Testing Decisions

- **What makes a good test**: assert on external behaviour (printed text, its order, return values and player state) through public interfaces. Tests should keep passing if Prompt's internals change.
- **Prompt (new seam, `CYOATests/PromptTests.cs`)**, driven by swapping the console as the existing suites do:
  - a valid answer returns its canonical value; mixed case and surrounding spaces still match.
  - an invalid answer prints the given invalid message, then the whole question again, then returns the next valid answer.
  - with `inventoryOf`, the hint and `Enter choice:` are printed, the Inventory command shows the Inventory and asks again without the invalid message, and nothing about the player changes.
  - without `inventoryOf`, the Inventory command is treated as invalid and no hint is printed.
  - `answerOnSameLine` prints `Enter choice:` without the hint.
  - input ending throws `EndOfInputException`.
  - at least one test in a translated language.
- **Combat (existing seam)**: build `Combat` directly with a player and dragon whose health is high enough that nobody dies, script an invalid answer then `r`, and assert there was exactly one attack round before the retreat. Written first, against the current code, so it fails before the fix.
- **Messages (existing seam)**: the old per-prompt method tests are rewritten against the one choice lookup. The key-coverage test is extended: every language has every map that English has, and each choice map's set of canonical values equals English's.
- **Whole-game tests stay**: `InventoryTests`, `EndOfInputTests`, `NorthPathTests`, `GameTests`, and the Merchant and Shrine `Visit` tests keep their assertions. Only their setup changes where signatures change. They are the evidence that nothing visible changed.
- **Removed**: the `GameTests` covering `Game.IsValid*`.
- **Prior art**: the console-swapping helpers in `MerchantTests` and `ShrineTests`, the key-coverage test in `MessagesTests`, and `CombatTests`.

## Out of Scope

- Injecting a console adapter instead of the static `Console` / `ConsoleInput` (architecture candidate 2). Prompt is the object that will receive it later.
- Translating the accepted words at the path, dragon, combat and roll prompts.
- Making the prompts uniform (one invalid message, `Enter choice:` everywhere, the Inventory at more prompts).
- Shrinking the duplicated per-prompt rows in `InventoryTests` and `EndOfInputTests`.
- Typed enum return values from Prompt, or a `PromptSpec` object bundling each prompt's keys.
- Injecting `Die` into `Game` and `Player`, or splitting character creation out of `Player`.

## Further Notes

- Use the glossary terms (Prompt, Inventory, Merchant, Shrine, Gold) in code, messages and tests.
- ADR-0001: all player text and accepted words stay in `language_data.json`, every language complete, no accented characters.
