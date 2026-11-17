# 01: Start out wearing Leather Armor

**What to build:** Every new character now starts out wearing Leather Armor, which absorbs 0 damage. Buying Magical Armor from the Merchant replaces it, so the player always wears exactly one armor and can no longer have none. The Character Summary and the combat stats screen name the armor the player is actually wearing ("Leather Armor (absorbs 0 damage)" or "Magical Armor (absorbs 3 damage)") instead of always saying Magical Armor. This works in English, Spanish, French and Italian. The Inventory display itself is built in ticket 02; this ticket provides the shared armor display it will use. See `.scratch/inventory-display/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [x] Armor has a kind and a name as well as its absorption. Alongside the existing Magical Armor factory there is a Leather Armor factory, with its 0 absorption held in a named constant.
- [x] A new player is wearing Leather Armor from the moment they are created. The player's armor is no longer optional, and code that checked for "no armor" stops doing so.
- [x] Buying Magical Armor from the Merchant replaces the Leather Armor. Prices, purchase rules and the purchase message are unchanged.
- [x] The dragon's attack always applies armor absorption. The "Your Magical Armor absorbs the blow!" message is unchanged and still shows only when damage after absorption is 0.
- [x] One shared way turns the player's armor into its translated display text, using a per-kind display message with the absorption as a placeholder. The Character Summary and the combat stats screen both use it.
- [x] Leather Armor is Armadura de Cuero / Armure de Cuir / Armatura di Cuoio. All new keys exist in all four languages with no accented characters (ADR-0001), and the existing same-keys-as-English test passes.
- [x] The Character Summary keeps all its existing lines, and the menu after character creation is unchanged.
- [x] Tests:
  - combat stats display shows the Leather Armor line for a new player and the Magical Armor line after buying it. The existing "without armor, no armor line" test is replaced.
  - Merchant purchase tests that expected no armor after a refused or non-armor purchase now expect Leather Armor.
  - one whole-game run that types `1` after character creation shows the Character Summary with "Armor: Leather Armor (absorbs 0 damage)".
  - existing dragon armor tests still pass.
