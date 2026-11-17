# 01: Shrine appears on the North path (walk past only)

**What to build:** When the player goes North, they now meet the Shrine (the Detour) on the way to the dragon. The game shows the journey text, then the Shrine's description and its three options, then, after the player chooses to walk past, a dragon arrival line, and then the existing dragon introduction, stats and prompt. Walking past changes nothing. This ticket sets up the Shrine end to end, but walking past is the only option it handles. Offer and pray are listed in the prompt but are handled in tickets 02 and 03; until then, choosing them prints the invalid-choice message and asks again. See `.scratch/shrine-detour/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] A new Shrine module, modelled on the Merchant, receives a die when it is created and has:
  - a choose operation that takes the player and a Shrine choice (Offer, Pray, Leave) and returns a result without writing to the console. Leave returns a walked-past result and leaves the player unchanged.
  - a visit operation that runs the console scene.
- [ ] The North narrative is split: the existing message now describes only the journey, and a new dragon-arrival message carries the "confronted with a dragon, hoard of gold behind it" text. This is done in English, Spanish, French and Italian.
- [ ] The North branch of the game runs, in order: journey text → Shrine visit → dragon arrival → existing dragon introduction, stats and prompt. It uses the game's die for the Shrine.
- [ ] The Shrine shows its description and an options prompt listing `o` (offer), `p` (pray) and `l` (leave).
- [ ] `l` and the full leave word are accepted in every language, case-insensitive and trimmed, through a new per-language Shrine choice map loaded by messages. The map already contains the offer and pray words for all four languages.
- [ ] Walking past prints a short leaving line and the player continues to the dragon.
- [ ] Invalid input prints the existing invalid-choice message and asks again.
- [ ] If input ends at the Shrine prompt, the game stops cleanly instead of looping (existing end-of-input behaviour).
- [ ] All new keys exist in all four languages with no accented characters (ADR-0001), and the existing same-keys-as-English test passes.
- [ ] Tests:
  - choose with Leave leaves the player unchanged.
  - visit accepts `l`, `leave`, a translated leave word, and mixed case or padded input.
  - visit handles invalid input followed by leave.
  - visit stops cleanly when input ends at the prompt.
  - the Italian Shrine choice map turns Italian words into the right choices.
  - one whole-game run that goes North and walks past checks the order of text: journey → Shrine → dragon arrival → dragon prompt.
