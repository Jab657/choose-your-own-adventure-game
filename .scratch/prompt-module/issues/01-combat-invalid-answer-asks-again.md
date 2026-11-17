# 01: Invalid answer at the combat prompt asks again

**What to build:** At the combat attack/retreat prompt, an answer that isn't attack or retreat (including `i`) prints the existing invalid message and shows the prompt again, without running another attack round. Today it falls through to the next round, so a typo attacks. Attack and retreat behave as before. This is the only player-visible change in the Prompt module feature, and it is fixed on the current code before the refactor. See `.scratch/prompt-module/spec.md`.

**Blocked by:** none

**Status:** ready-for-agent

- [ ] Written test-first: the test fails on the current code.
- [ ] Test (`CombatTests`): build `Combat` directly with a player and dragon whose health is high enough that nobody dies; script an invalid answer then `r`. Assert exactly one attack round ran before the retreat, the invalid message was printed, and the prompt was shown again.
- [ ] Test: `i` at the combat prompt gets the invalid message and no Inventory.
- [ ] An invalid answer prints `invalid` and asks again with no extra attack round.
- [ ] Input ending at the combat prompt still stops the game cleanly.
