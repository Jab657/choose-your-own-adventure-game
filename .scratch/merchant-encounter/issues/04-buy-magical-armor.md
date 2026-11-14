# 04: Buy Magical Armor

**What to build:** At Oswin's stall, choosing `a`/`armor` buys Magical Armor for 15 Gold. The armor then shows in the character and combat stats, and in the dragon fight every hit that gets past the player's defense is reduced by 3, never below 0. When the armor absorbs a whole hit, the player sees "your magical armor absorbs the blow" and loses no Health Points. This ticket also establishes the Merchant's shared purchase rules: not enough Gold is refused and returns to the menu, and only one purchase is allowed. See the spec at `.scratch/merchant-encounter/spec.md` (Armor on the player, Merchant module, Armor in combat).

**Blocked by:** 01 (Make dice controllable in tests), 03 (Meet Oswin on the South path)

**Status:** ready-for-agent

- [ ] The Merchant has a purchase operation taking the player and an item, returning Bought, NotEnoughGold or AlreadyTraded; player state changes only on Bought.
- [ ] Buying armor reduces Gold by 15, gives the player Magical Armor (absorbs 3), and marks the Merchant as traded.
- [ ] With less than 15 Gold, the purchase returns NotEnoughGold and Gold and armor are unchanged; in the visit, the not-enough-Gold message is shown and the menu re-prompts.
- [ ] After any purchase, a further purchase returns AlreadyTraded.
- [ ] `a` / `armor` (and the translated full word in each language) buys armor in the visit.
- [ ] Character stats and combat stats show an Armor line only when the player has armor.
- [ ] In the dragon's attack, damage that gets past the player's defense is reduced by 3 and floored at 0.
- [ ] A fully absorbed hit shows the "absorbs the blow" message instead of the damage line, and Health Points are unchanged.
- [ ] A player without armor takes exactly the rolled damage, as before.
- [ ] Tests use the scripted die from ticket 01 for the combat cases, call the purchase operation directly for the rule cases, and use scripted console input for the visit.
- [ ] All new message keys exist in all four languages.
