# 02: Load every language map with one loader

**What to build:** A prefactor with no visible change. The text module loads every map section of the chosen language through one loader instead of one copy-pasted loop per map, display maps included. Every map is string to string: the Shrine choice map is no longer turned into the Shrine's choice type inside the text module, so the Shrine converts the canonical value itself. One choice lookup (map name + answer → canonical value, or nothing) is added alongside the existing per-prompt methods, ready for the Prompt module. The coverage test is extended so a missing or inconsistent map is caught before it can crash the game at language selection. See `.scratch/prompt-module/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [x] One loader reads every map section; adding a map to the language data needs no new loading code.
- [x] The Shrine choice map is string to string; the Shrine (or the existing Shrine choice method) converts to the Shrine's choice type, and Shrine behaviour is unchanged.
- [x] A single choice lookup exists, trimming and ignoring case like today's methods. The existing per-prompt methods stay for now.
- [x] Coverage test extended: every language has every map English has, and each choice map's set of canonical values equals English's. It fails if a map is removed from any language.
- [x] All existing tests pass unchanged.
