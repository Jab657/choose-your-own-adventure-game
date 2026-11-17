# 01: Invalid answer at the combat prompt asks again

**What to build:** At the combat attack/retreat prompt, an answer that isn't attack or retreat (including `i`) prints the existing invalid message and shows the prompt again, without running another attack round. Today it falls through to the next round, so a typo attacks. Attack and retreat behave as before. This is the only player-visible change in the Prompt module feature, and it lands on the current code before combat moves onto Prompt. See `.scratch/prompt-module/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] Written test-first: the new tests fail on the current code.
- [ ] Combat test, building combat directly with a player and dragon whose health is high enough that nobody dies: an invalid answer then retreat gives exactly one attack round before the retreat, prints the invalid message, and shows the prompt again.
- [ ] Combat test: `i` at the combat prompt gets the invalid message and no Inventory.
- [ ] Attack and retreat at the combat prompt behave as before.
- [ ] Input ending at the combat prompt still stops the game cleanly.
