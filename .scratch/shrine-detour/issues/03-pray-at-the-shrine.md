# 03: Pray at the Shrine

**What to build:** At the Shrine, the player can pray for free, as a gamble. A d6 roll on the Shrine's die decides it: 1–2 means the spirits are displeased and the player loses 2 Health Points, but never drops below 1; 3–6 means the prayer is answered and the player gains 2 Health Points. The game then shows the outcome and the player's new Health Points before they continue to the dragon. See `.scratch/shrine-detour/spec.md`.

**Blocked by:** 01 (Shrine appears on the North path)

**Status:** resolved

- [x] choose with Pray rolls a d6 on the Shrine's die.
- [x] Rolls of 1–2 return a prayer-displeased result and take 2 Health Points, flooring at 1.
- [x] Rolls of 3–6 return a prayer-answered result and add 2 Health Points, with no upper cap.
- [x] Praying never changes Gold.
- [x] visit accepts `p`, `pray`, and the translated pray word in each language, case-insensitive and trimmed.
- [x] After praying, visit prints the matching outcome text and the new Health Points, then the player continues to the dragon.
- [x] All new keys exist in English, Spanish, French and Italian with no accented characters (ADR-0001), and the existing same-keys-as-English test passes.
- [x] Tests, using the scripted die:
  - choose with rolls 1, 2, 3 and 6.
  - a bad prayer at 1 and at 2 Health Points ends at 1.
  - visit with short and full words prints the right outcome and Health Points line for an answered prayer and for a displeased one.
