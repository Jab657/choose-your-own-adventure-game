# Spec: Merchant encounter on the South path

Status: ready-for-agent

## Problem Statement

When a player takes the South path, nothing happens: the narrative says they get bored and go home, yet the game quietly returns them to the path choice. The South path is a dead end with no effect on the game, and the only real content is the dragon fight in the North. Players have no way to prepare for that fight or make a meaningful choice before it.

## Solution

On their first trip South, the player meets **Oswin**, a **Merchant**. Every character now starts with a fixed 20 **Gold**. Oswin offers exactly one purchase: **Magical Armor** (15 Gold), which absorbs 3 damage from every dragon hit that gets past the player's defense, or a **Magical Weapon** (20 Gold), which enchants the player's existing occupation weapon for +4 maximum damage. The player may also leave without buying. Either way, the Merchant is then gone: later trips South show an empty stall. After the encounter, the player returns to the path choice and can head North to face the dragon with their new item.

All new text is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want my character to start with Gold, so that I have something to spend at the Merchant.
2. As a player, I want every character to start with the same amount of Gold, so that the Merchant's offer is fair regardless of my rolls.
3. As a player, I want to see my Gold in my character stats, so that I know what I can afford.
4. As a player, I want the South path to lead somewhere interesting, so that choosing it is worthwhile.
5. As a player, I want to meet a named Merchant, Oswin, on my first trip South, so that the encounter feels like part of the story.
6. As a player, I want Oswin to list both items with their prices, so that I can make an informed choice.
7. As a player, I want the Merchant's offer to show my current Gold, so that I don't have to check my stats separately.
8. As a player, I want to buy Magical Armor, so that I take less damage from the dragon.
9. As a player, I want to buy a Magical Weapon, so that I deal more damage to the dragon.
10. As a player, I want the Magical Weapon to be an enchanted version of my own weapon, so that my character's occupation still makes sense (a Magician keeps their spell, a Thief keeps their dagger).
11. As a player, I want to leave the Merchant without buying anything, so that I'm never forced into a purchase.
12. As a player, I want to choose using short or full words (`a`/`armor`, `w`/`weapon`, `l`/`leave`), so that the Merchant works like the dragon prompt I already know.
13. As a player, I want the Merchant's choices to work in my language, so that I can play fully in Spanish, French or Italian.
14. As a player, I want my input to be accepted regardless of capitalisation or surrounding spaces, so that small typing differences don't trip me up.
15. As a player, I want an invalid choice to tell me so and ask again, so that a typo doesn't end the encounter.
16. As a player, I want a purchase to happen as soon as I choose it, so that the encounter is quick.
17. As a player, I want to be told clearly when I don't have enough Gold for an item, so that I understand why the purchase didn't happen.
18. As a player, I want to stay at the Merchant's menu after being refused, so that I can pick the other item or leave.
19. As a player, I want my Gold to go down by the item's price when I buy, so that the purchase feels real.
20. As a player, I want to be able to buy only one item, so that the choice between armor and weapon matters.
21. As a player, I want to return to the path choice after the Merchant, so that I can take my new item North to the dragon.
22. As a player, I want later trips South to show that Oswin's stall is empty, so that the world stays consistent.
23. As a player, I want leaving without buying to also end the Merchant's visit, so that walking away is a real decision.
24. As a player who bought Magical Armor, I want it shown in my character stats, so that I can confirm I have it.
25. As a player who bought Magical Armor, I want it shown in the combat stats, so that I can see it during the fight.
26. As a player who bought Magical Armor, I want each dragon hit that gets through to be reduced by 3, so that the armor actually protects me.
27. As a player who bought Magical Armor, I want a message when the armor absorbs an entire hit, so that I understand why I took no damage.
28. As a player who bought Magical Armor, I want damage never to go below zero, so that armor never heals me.
29. As a player without armor, I want the dragon fight to work exactly as before, so that the change doesn't alter the base game.
30. As a player who bought a Magical Weapon, I want my weapon shown with its magical name in my stats and attack text, so that I can see the enchantment.
31. As a player who bought a Magical Weapon, I want my weapon's maximum damage shown with the +4 bonus, so that I know how strong it is.
32. As a Fighter or Magician, I want my enchanted weapon to reach 16 maximum damage, so that the upgrade is meaningful.
33. As a Thief, I want my enchanted dagger to reach 10 maximum damage, so that my weaker weapon still benefits.
34. As an Archer, I want my enchanted bow to reach 12 maximum damage, so that the upgrade is meaningful.
35. As a Spanish, French or Italian player, I want the magical weapon names to be grammatically correct in my language, so that the text reads naturally.
36. As a player in any language, I want never to see raw placeholder text like `[merchant_intro]`, so that the game feels finished.
37. As a developer, I want the Merchant's buying rules testable without the console, so that edge cases like "not enough Gold" can be verified even though normal play never reaches them.
38. As a developer, I want dice rolls to be controllable in tests, so that armor damage reduction can be verified exactly.
39. As a developer, I want a test that fails if any language is missing a key, so that translation gaps are caught before release.

## Implementation Decisions

- **Gold on the player**: the player gains a Gold amount, set to a fixed 20 at character creation (no roll, no race/occupation modifier). It appears in the character stats but not the combat stats. Gold has no use other than the Merchant; defeating the dragon does not add Gold.
- **Armor on the player**: the player gains an optional armor slot. When present, it is Magical Armor with an absorption value of 3. The character stats and combat stats show an Armor line only when the player has armor.
- **Merchant module (new)**: a deep module that owns all Merchant rules and the once-per-game state. Its interface:
  - A purchase operation taking the player and an item (Magical Armor or Magical Weapon), returning one of: Bought, NotEnoughGold, AlreadyTraded. Gold, armor and weapon change only on Bought.
  - A "has traded" state that becomes true after a purchase **or** after the player leaves.
  - A visit operation taking the player and messages that runs the console encounter: if the Merchant has already traded it shows the empty stall; otherwise it shows Oswin's introduction, both items with prices, and the player's Gold, then loops on input until a purchase or leave.
- **Prices**: Magical Armor costs 15 Gold; Magical Weapon costs 20 Gold. With 20 starting Gold, both are always affordable in normal play; the NotEnoughGold path exists for correctness and is covered by unit tests.
- **Enchanting**: buying the Magical Weapon keeps the player's current weapon, adds +4 to its maximum damage, and switches its type to the magical variant so its translated display name changes (long sword → magical long sword, and so on for the lightning bolt spell, dagger and long bow). The ASCII art is unchanged.
- **Armor in combat**: in the dragon's attack, after a hit gets past the player's defense, damage is reduced by the armor's absorption value and floored at 0. When the result is 0, the "absorbs the blow" message is shown instead of the normal damage line, and Health Points do not change. Players without armor are unaffected.
- **Merchant menu input**: `a`/`armor`, `w`/`weapon`, `l`/`leave`, case-insensitive and trimmed, with the full words translated per language (via a language map, like the existing race and occupation maps). Invalid input prints the existing invalid-choice message and re-prompts. No confirmation step.
- **Game loop**: the South branch calls the Merchant's visit on every trip and then returns to the path choice. The old "bored, return home" South narrative is replaced by the Merchant introduction. One Merchant instance lives for the whole game.
- **Translations**: every new message key (Merchant intro, offer lines, prices and Gold display, purchase confirmations, not-enough-Gold, empty stall, leave, armor absorbs the blow, Gold and Armor stats labels) is added to all four languages. The weapon display map in each language gets full translated names for the four magical weapons, rather than a "magical {0}" pattern, so adjective order and gender agreement are correct. The merchant's name, Oswin, is not translated.
- **Dice substitution**: the die's roll operation becomes overridable so tests can supply a scripted die. The dragon already receives its die from outside, so no other wiring changes are needed. The player continues to create its own die; nothing requires changing that.

## Testing Decisions

- **What makes a good test**: assert on external behavior (return values, player Gold/armor/weapon state, Health Points, printed messages) through the public interfaces of the modules below, not on private methods or internal structure. Tests should keep passing if the internals are refactored.
- **Merchant (new seam, primary)**: test the purchase operation directly for buying armor (Gold −15, armor present), buying the weapon (Gold −20, +4 max damage, magical type) for each of the four occupations, NotEnoughGold leaving everything unchanged, and AlreadyTraded refusing a second purchase. Test the visit operation with scripted console input for: buying via short and full words, case and whitespace tolerance, invalid input re-prompting, refusal followed by a different choice, leave buying nothing and marking the Merchant as traded, and a second visit showing the empty stall.
- **Dragon attack (existing seam)**: with a scripted die, verify that armor reduces a hit by exactly 3, that a hit of 3 or less results in 0 damage, unchanged Health Points and the "absorbs the blow" message, and that a player without armor takes the full rolled damage as before.
- **Messages (existing seam)**: extend the existing same-keys-as-English check so every language has every key English has, and verify each magical weapon name translates to a non-placeholder value in all four languages.
- **Prior art**: the combat and dragon tests already build players and dragons with object initialisers and a real die; the new tests follow the same style with a scripted die. The existing Italian same-keys-as-English test is the model for translation completeness. Console-input tests are new to this suite; they redirect console input and output for the duration of the test.
- **Not tested**: the one-line South branch change in the main game loop, consistent with how the game loop is currently untested.

## Out of Scope

- Fixing the infinite "Invalid choice" loop when console input ends (end-of-input returns null). This affects every prompt in the game and will be handled as a separate ticket; the Merchant prompt behaves like existing prompts.
- Changing how the player's die is created (injecting a die into the player).
- Any other use of Gold: earning it, dragon hoard rewards, or other shops.
- Buying more than one item, selling items, or haggling.
- Merchant ASCII art or additional dialogue.
- Rebalancing the dragon's stats.

## Further Notes

- Domain terms (Gold, Merchant, Magical Armor, Magical Weapon) are defined in the repo glossary; use them consistently in code, messages and tests.
- ADR-0001 (Italian language support) establishes that every supported language must be complete; this spec follows that by requiring all four languages at launch.
- The South narrative previously claimed the player went home while the code returned them to the path choice; this spec resolves the mismatch in favor of returning to the path choice.
