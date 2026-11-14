# 03: Meet Oswin on the South path

**What to build:** On the first trip South, the player meets Oswin, the Merchant, instead of the old "bored, return home" narrative. Oswin gives an introduction and lists Magical Armor (15 Gold) and the Magical Weapon (20 Gold) alongside the player's current Gold. In this ticket only leaving works: choosing `l`/`leave` ends the visit and returns the player to the path choice. The Merchant is then gone for the rest of the game; later trips South show an empty stall. See the spec at `.scratch/merchant-encounter/spec.md` (Merchant module, Merchant menu input, Game loop).

**Blocked by:** 02 (Starting Gold)

**Status:** ready-for-agent

- [ ] A Merchant module owns the once-per-game state and a visit operation; the game creates one Merchant for the whole game and calls its visit on every South trip.
- [ ] The first visit shows Oswin's introduction, both items with prices, and the player's Gold.
- [ ] `l` / `leave` (and the translated full word in each language) ends the visit, buys nothing, and marks the Merchant as traded.
- [ ] Input is case-insensitive and trimmed; invalid input shows the existing invalid-choice message and re-prompts.
- [ ] A later visit shows the empty-stall message and returns immediately.
- [ ] After any visit, the player is back at the path choice and can go North.
- [ ] The old South narrative is no longer shown.
- [ ] All new message keys exist in English, Spanish, French and Italian; the name "Oswin" is not translated.
- [ ] Tests drive the visit operation with scripted console input to cover: offer shown, leave via short and full word, case/whitespace tolerance, invalid input re-prompting, and the empty stall on a second visit.
