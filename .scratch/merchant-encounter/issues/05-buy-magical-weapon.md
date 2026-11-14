# 05: Buy Magical Weapon

**What to build:** At Oswin's stall, choosing `w`/`weapon` buys a Magical Weapon for 20 Gold: the player's own occupation weapon is enchanted, gaining +4 maximum damage and a magical name. Fighter and Magician reach 16, Thief 10, Archer 12. The magical name shows in the character stats, combat stats and attack text, correctly translated in every language. Reuses the purchase rules from ticket 04. See the spec at `.scratch/merchant-encounter/spec.md` (Enchanting, Translations).

**Blocked by:** 04 (Buy Magical Armor), for the shared purchase rules only.

**Status:** ready-for-agent

- [ ] Buying the weapon reduces Gold by 20, keeps the player's existing weapon, adds +4 to its maximum damage, and switches it to its magical variant; ASCII art is unchanged.
- [ ] Verified for all four occupations: long sword 12→16, lightning bolt spell 12→16, dagger 6→10, long bow 8→12.
- [ ] With less than 20 Gold, the purchase returns NotEnoughGold and Gold and weapon are unchanged.
- [ ] After any purchase, a further purchase returns AlreadyTraded.
- [ ] `w` / `weapon` (and the translated full word in each language) buys the weapon in the visit.
- [ ] Each language's weapon display map has a full, grammatically correct name for each of the four magical weapons (no "magical {0}" pattern).
- [ ] A test verifies every magical weapon name translates to a non-placeholder value in all four languages.
- [ ] Stats and attack text show the magical name and the new maximum damage.
