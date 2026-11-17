# 04: Delete the unused Game.IsValid* methods

**What to build:** Delete `Game.IsValidMenuChoice`, `Game.IsValidPathChoice` and `Game.IsValidCombatChoice`. The game never calls them, and after tickets 02 and 03 the accepted answers live only in the choice maps, so these copies would drift. Delete the `GameTests` that cover them. See `.scratch/prompt-module/spec.md`.

**Blocked by:** 03 (Move the remaining prompts onto Prompt)

**Status:** ready-for-agent

- [ ] The three methods are deleted.
- [ ] Their tests in `GameTests` are deleted; no other test depends on them.
- [ ] The build and the full test suite pass.
