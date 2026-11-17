# 03: Check the Inventory at the Merchant, Shrine and dragon prompts

**What to build:** The Inventory command and hint from ticket 02 now also work at the Merchant prompt, the Shrine prompt and the dragon-encounter prompt. Typing `i` or the full word shows the Inventory, then the same prompt again (only the prompt, not the Merchant or Shrine intro text), and the player carries on with their choice as normal. Nothing changes. The in-combat attack/retreat prompt and the menu after character creation do not accept the command; in combat, `i` gets the existing invalid-choice message. See `.scratch/inventory-display/spec.md`.

**Blocked by:** 02 (Check the Inventory at the path prompt)

**Status:** ready-for-agent

- [ ] The Merchant, Shrine and dragon-encounter prompts each check for the Inventory command before their own choices. On a match they display the Inventory and show the same prompt again, without the invalid-choice message and without changing any state.
- [ ] The hint message from ticket 02 is printed after each of these three prompt lines, before "Enter choice:". The prompt messages themselves are not reworded.
- [ ] The combat attack/retreat prompt and the menu after character creation are unchanged.
- [ ] If input ends at any of these prompts, the game still stops cleanly.
- [ ] Tests (whole-game runs on scripted input):
  - South → `i` at the Merchant prompt shows the Inventory and the Merchant prompt again; leaving afterwards works.
  - North → `i` at the Shrine prompt shows the Inventory and the Shrine prompt again; walking past afterwards works.
  - North → walk past → `i` at the dragon-encounter prompt shows the Inventory and the dragon prompt again; retreating afterwards works.
  - the hint appears under each of the three prompts.
  - at least one of these runs uses a translated full word.
