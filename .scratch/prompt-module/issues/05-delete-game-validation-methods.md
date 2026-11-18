# 05: Delete the unused game validation methods

**What to build:** Remove the game's menu, path and combat choice validation methods. Nothing in the game calls them, and they repeat the accepted answers that are moving into the language data, so they would silently disagree with the choice maps. Remove their tests too. See `.scratch/prompt-module/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [x] The three validation methods are deleted.
- [x] Their tests are deleted; no other test depends on them.
- [x] The build and the full test suite pass.
