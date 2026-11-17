# 02: Make an offering at the Shrine

**What to build:** At the Shrine, the player can make an offering: it costs 5 Gold and always restores 5 Health Points, and afterwards the game shows the player's new Health Points and remaining Gold before they continue to the dragon. A player with less than 5 Gold (for example, after buying the Magical Weapon) is told the offering costs 5 and how much Gold they have, and is asked again so they can pray or walk past instead. See `.scratch/shrine-detour/spec.md`.

**Blocked by:** 01 (Shrine appears on the North path)

**Status:** resolved

- [x] choose with Offer and at least 5 Gold takes 5 Gold, adds 5 Health Points, and returns an offering-made result.
- [x] choose with Offer and exactly 5 Gold succeeds and leaves 0 Gold.
- [x] choose with Offer and less than 5 Gold returns a not-enough-Gold result and leaves the player's Gold and Health Points unchanged.
- [x] Healing has no upper cap.
- [x] visit accepts `o`, `offer`, and the translated offer word in each language, case-insensitive and trimmed.
- [x] After a successful offering, visit prints the offering outcome, the new Health Points and the remaining Gold, then the player continues to the dragon.
- [x] When the player can't afford the offering, visit prints a not-enough-Gold message showing the cost and the player's current Gold, then asks again.
- [x] All new keys exist in English, Spanish, French and Italian with no accented characters (ADR-0001), and the existing same-keys-as-English test passes.
- [x] Tests cover:
  - choose: enough Gold, exactly 5, too little.
  - visit: buying via the short and full words; outcome and stats lines printed; an unaffordable offering followed by walking past, which prints the refusal and ends with the player unchanged.
