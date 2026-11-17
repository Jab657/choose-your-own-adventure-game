# 02: Prompt module, used by the Inventory prompts

**What to build:** Add the Prompt module and move the four prompts that offer the Inventory onto it: path, Merchant, Shrine and dragon encounter. `Messages` loads every map in one loop and exposes one choice lookup that Prompt uses. The path and dragon answers move from C# into new `pathChoiceMap` and `dragonChoiceMap` sections in every language, holding today's English words. Nothing the player sees changes. See `.scratch/prompt-module/spec.md` for the interface and the wiring table.

**Blocked by:** 01 (Invalid answer at the combat prompt asks again)

**Status:** ready-for-agent

- [ ] `Prompt(Messages)` with `Ask(promptKey, choiceMapName, invalidKey, inventoryOf, answerOnSameLine)` as in the spec. Reads through the existing `ConsoleInput.ReadLine()`.
- [ ] `Messages.ReadDictionary` loads every `*Map` section in one loop; all maps are string → string. One choice lookup is added for Prompt.
- [ ] `pathChoiceMap` (`north`/`n`/`south`/`s`/`exit`/`e`) and `dragonChoiceMap` (`attack`/`a`/`retreat`/`r`) added to English, Spanish, French and Italian with the same English words and canonical values.
- [ ] `Game` creates one Prompt after language selection. Path and dragon prompts use it; `PromptForPath`'s loop is gone.
- [ ] `Merchant.Visit` and `Shrine.Visit` take the Prompt and use it, looping themselves on "not enough Gold". Merchant converts the canonical string to `MerchantItem`, Shrine to `ShrineChoice`.
- [ ] `IsInventoryCommand`, `NormalizeMerchantChoice` and `NormalizeShrineChoice` are deleted. Their test cases (including translated answers) move up to `PromptTests`; the choice lookup is tested only through Prompt.
- [ ] Call sites use named arguments.
- [ ] Tests:
  - new `PromptTests` covering every Prompt case listed in the spec's Testing Decisions.
  - key-coverage test extended: every language has every map English has, and each choice map's canonical values equal English's.
  - existing `InventoryTests`, `EndOfInputTests`, `NorthPathTests`, Merchant and Shrine tests pass with only setup changes.
