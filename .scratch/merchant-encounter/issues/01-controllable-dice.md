# 01: Make dice controllable in tests

**What to build:** Tests can supply dice that return chosen rolls instead of random ones, so combat outcomes can be verified exactly. The die's roll operation becomes overridable, and the test project gains a scripted die that returns a pre-set sequence of rolls. Game behavior is unchanged. See the spec at `.scratch/merchant-encounter/spec.md` (Implementation Decisions: Dice substitution).

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] The die's roll operation can be overridden by a test double; normal games still roll randomly.
- [ ] The test project has a scripted die that returns a given sequence of rolls in order.
- [ ] A test scripts a dragon attack (attack roll hits, defense roll fails, a chosen damage roll) and asserts the player's Health Points drop by exactly that damage.
- [ ] The player still creates its own die; no change to how the player is constructed.
- [ ] All existing tests pass.
