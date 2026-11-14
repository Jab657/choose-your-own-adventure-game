# 01: Stop prompts looping forever when input ends

**What to build:** When console input ends (for example Ctrl+Z, or a piped script running out of lines), reading input returns null and every prompt in the game currently prints "Invalid choice" forever. Instead, the game should stop cleanly when input ends, at any prompt: language selection, main menu, character creation, rolls, path choice, dragon encounter, combat, and the Merchant. Raised during the merchant-encounter design and deliberately kept out of that spec (`.scratch/merchant-encounter/spec.md`, Out of Scope).

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] At every prompt, end of input ends the game cleanly instead of looping.
- [ ] Behavior for normal input, including invalid-choice re-prompting, is unchanged.
- [ ] A test feeds a console input that ends early and asserts the game returns rather than hanging.
- [ ] All existing tests pass.
