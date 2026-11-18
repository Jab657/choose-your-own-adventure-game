# 03: Prompt module, used by the four Inventory prompts

**What to build:** The Prompt module, and the four prompts that offer the Inventory moved onto it: path, Merchant, Shrine and dragon encounter. The game creates one Prompt after the language is chosen and passes it to the Merchant visit, the Shrine visit and the dragon encounter. The path and dragon answers move from game logic into new path and dragon choice maps in all four languages, holding today's English words. The Merchant and Shrine loop around Prompt themselves on "not enough Gold". Nothing the player sees changes. See `.scratch/prompt-module/spec.md` for the "ask" operation and the wiring table.

**Blocked by:** 02 (Load every language map with one loader)

**Status:** resolved

- [x] Prompt's "ask" behaves as the spec describes: question, optional Inventory hint, optional "Enter choice:", trim and lowercase, free Inventory check when offered, canonical value on a valid answer, invalid message and ask again otherwise, end-of-input signal when input ends.
- [x] Prompt tests, through its interface: valid answer; mixed case and spaces; invalid then valid; Inventory offered (hint, "Enter choice:", Inventory shown, asked again, player unchanged); Inventory not offered (command invalid, no hint); same-line flag; input ending; translated answers.
- [x] Path and dragon choice maps added to English, Spanish, French and Italian with the same English words and canonical values; the coverage test from 02 covers them.
- [x] Path, Merchant, Shrine and dragon prompts use Prompt; their own loops and the four copies of the Inventory check are gone. Call sites use named arguments.
- [x] The text module's Inventory-command, Merchant-choice and Shrine-choice methods are deleted; their test cases move to the Prompt tests.
- [x] The whole-game Inventory, end-of-input and North path tests, and the Merchant and Shrine visit tests, pass with only setup changes.
