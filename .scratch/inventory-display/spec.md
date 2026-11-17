# Spec: Inventory display

Status: ready-for-agent

## Problem Statement

A player has no quick way to check what they are carrying while they play. The only place their weapon, armor and Gold are shown is the Character Summary, which is offered once, right after character creation. After that, a player deciding whether to buy from the Merchant, make an offering at the Shrine or fight the dragon has to remember how much Gold they have left and what they bought.

The armor shown on screen is also wrong. A player who never bought Magical Armor has no armor line at all, and the Character Summary and combat stats screen always describe whatever armor the player has as "Magical Armor".

## Solution

At any of the game's four main decision points (the path prompt, the Merchant, the Shrine and the dragon encounter), the player can type `i`, or the word for "inventory" in their language, to check their **Inventory**: their weapon, its maximum damage, their armor and their Gold. Checking is free. Nothing changes, and the same prompt is shown again afterwards. Each of those prompts reminds the player that `i` is available.

Every character now starts out wearing **Leather Armor**, which absorbs no damage. Buying Magical Armor from the Merchant replaces it. The Inventory, Character Summary and combat stats screen always name the armor the player is actually wearing.

All new text is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want to check my Inventory while I play, so that I don't have to remember what I'm carrying.
2. As a player, I want to check my Inventory at the path prompt, so that I can decide between North and South knowing what I have.
3. As a player, I want to check my Inventory at the Merchant, so that I can see my Gold and gear before I choose what to buy.
4. As a player, I want to check my Inventory at the Shrine, so that I can see whether I can afford an offering.
5. As a player, I want to check my Inventory before I fight or flee the dragon, so that I can judge how well equipped I am.
6. As a player, I want to type the short code `i` to check my Inventory, so that checking is quick.
7. As a player, I want `i` to work in every language, so that the short code is the same wherever I play.
8. As a player, I want to type the full word for "inventory" in my language (`inventory`, `inventario`, `inventaire`, `inventario`), so that the command feels natural.
9. As a player, I want the Inventory command to work regardless of capitals or surrounding spaces, so that small typing differences don't trip me up.
10. As a player, I want checking my Inventory to cost nothing and change nothing, so that I can check as often as I like.
11. As a player, I want the same prompt shown again after I check my Inventory, so that I can carry on with the choice I was making.
12. As a player, I want each of the four prompts to tell me I can type `i` to check my Inventory, so that I find out the command exists.
13. As a player, I want the hint in my own language, so that I can play fully in Spanish, French or Italian.
14. As a player, I want the Inventory to have a clear heading, so that I can tell it apart from the story text around it.
15. As a player, I want the Inventory heading to look like the Character Summary heading, so that the game's screens feel consistent.
16. As a player, I want to see my weapon in my Inventory, so that I know what I'm fighting with.
17. As a player, I want to see my weapon's maximum damage in my Inventory, so that I can judge how strong it is.
18. As a player who bought the Magical Weapon, I want my Inventory to show the enchanted weapon and its higher damage, so that I can see what I paid for.
19. As a player, I want to see my armor in my Inventory, with how much damage it absorbs, so that I know how protected I am.
20. As a player, I want to see my Gold in my Inventory, so that I know what I can afford.
21. As a player who has spent all my Gold, I want my Inventory to show 0 Gold rather than leaving the line out, so that I know I have none.
22. As a player, I want the Inventory lines to use the same wording as the Character Summary, so that the same things are called the same names everywhere.
23. As a player, I want my Inventory to reflect what I've bought and spent, so that it always shows what I have right now.
24. As a player, I want to start out wearing Leather Armor, so that my character isn't going into the mountains unprotected.
25. As a player, I want Leather Armor to show that it absorbs 0 damage, so that I understand it doesn't protect me in combat.
26. As a player, I want Magical Armor to replace my Leather Armor when I buy it, so that I'm wearing one armor at a time.
27. As a player who bought Magical Armor, I want my Inventory to show Magical Armor and its absorption, so that I can see what I paid for.
28. As a player, I want the Character Summary to name the armor I'm actually wearing, so that it never claims I have Magical Armor when I don't.
29. As a player, I want the combat stats screen to name the armor I'm actually wearing, so that I know my real protection going into the fight.
30. As a player in Spanish, French or Italian, I want Leather Armor shown in my language (Armadura de Cuero, Armure de Cuir, Armatura di Cuoio), so that the game feels finished.
31. As a player in Spanish, French or Italian, I want all the new text to follow the game's style (no accented characters), so that it displays correctly in my console.
32. As a player, I want the Merchant's purchase message to stay the same when Magical Armor replaces my Leather Armor, so that buying armor still reads naturally.
33. As a player, I want the Character Summary I'm offered after character creation to keep its existing lines, so that it still shows my full character.
34. As a player, I want the menu after character creation to stay as it is, so that starting the adventure works the way it always has.
35. As a player, I want typing `i` during a combat round to be treated as an invalid choice, so that the fight keeps moving and the Inventory doesn't interrupt it.
36. As a player whose input ends at any of these prompts, I want the game to stop cleanly, so that it doesn't loop forever.
37. As a player in any language, I want never to see raw placeholder text like `[inventory_header]`, so that the game feels finished.
38. As a developer, I want the Inventory testable through a scripted whole-game run, so that it's checked the way a player sees it.
39. As a developer, I want the existing key-coverage test to fail if any language is missing an Inventory or Leather Armor key, so that translation gaps are caught before release.
40. As a developer, I want "the player has no armor" to no longer be possible, so that the code doesn't carry a case the game can't reach.

## Implementation Decisions

- **Domain terms**: **Inventory** (weapon, armor and Gold) and **Leather Armor** (starting armor that absorbs no damage, replaced by Magical Armor) are defined in the glossary. The Inventory has no size or weight limit.
- **Armor module**: armor gains a kind and a name as well as its absorption. Alongside the existing Magical Armor factory there is a Leather Armor factory with 0 absorption. Absorption behaviour is unchanged. Leather Armor is a placeholder and may get a real effect later, so its absorption is a named constant like Magical Armor's.
- **Player**: every player is wearing Leather Armor from the moment they are created. The player's armor is no longer optional: there is always exactly one armor. Code that checked for "no armor" stops doing so.
- **Merchant**: buying Magical Armor replaces the Leather Armor. The purchase rules, prices and purchase message are unchanged.
- **Dragon attack**: armor absorption is always applied. The "Your Magical Armor absorbs the blow!" message is left as is: it shows when damage after absorption is 0, which Leather Armor can never cause because a damage roll is always at least 1.
- **Armor display**: one shared way of turning the player's armor into its translated display text ("Leather Armor (absorbs 0 damage)" / "Magical Armor (absorbs 3 damage)"), using a per-kind display message with the absorption as a placeholder. The Inventory, Character Summary and combat stats screen all use it, replacing the hard-coded Magical Armor text in the last two.
- **Inventory display**: the player can display their Inventory. It prints, in order: a translated `----- Inventory -----` heading (styled like the Character Summary heading), then the weapon, its maximum damage, the armor and the Gold, each using the Character Summary's existing labels. Gold always shows, including 0. The Character Summary keeps all its existing lines.
- **Inventory command**: messages gain a check for whether an input is the Inventory command in the current language. Each language section gets a new Inventory command map listing `i` and the full word (`inventory` / `inventario` / `inventaire` / `inventario`), loaded like the existing Merchant and Shrine choice maps. Input is trimmed and compared without regard to case.
- **Prompts**: the path prompt, the Merchant prompt, the Shrine prompt and the dragon-encounter prompt each check for the Inventory command before their own choices. On a match, they display the Inventory and loop back to show the same prompt again, without printing the invalid-choice message or changing any state. The Merchant and Shrine intro text is not repeated, only the prompt.
- **Hint**: one new translated hint message ("Enter 'i' to check your inventory.") is printed after each of the four prompt lines, before "Enter choice:". The existing prompt messages are not reworded.
- **Not included**: the menu after character creation ("1. List stats / 2. Proceed") and the in-combat attack/retreat prompt don't accept the Inventory command. In combat, `i` gets the existing invalid-choice message.
- **Translations**: every new key (Inventory heading, hint, Leather Armor display) and the Inventory command map are added to English, Spanish, French and Italian, with no accented characters, per ADR-0001. Leather Armor is Armadura de Cuero / Armure de Cuir / Armatura di Cuoio.
- **End of input**: all prompts already read through the shared console input, so input ending at any of them still stops the game cleanly.

## Testing Decisions

- **What makes a good test**: assert on external behaviour (printed text, its order, and player state) through public interfaces, not on private methods or internal structure. Tests should keep passing if the internals are refactored.
- **Whole game, scripted console input (existing seam, primary)**: the Inventory doesn't depend on dice (the weapon comes from the occupation, Gold starts at a fixed amount and Leather Armor is fixed), so whole-game runs give the same result every time:
  - `i` at the path prompt for a new Fighter shows the heading, long sword, 12, Leather Armor (absorbs 0 damage) and 20 Gold, then the path prompt again.
  - South → buy Magical Armor → `i` at the path prompt shows Magical Armor (absorbs 3 damage) and 5 Gold.
  - South → buy the Magical Weapon → `i` shows the magical weapon, its higher damage and 0 Gold.
  - `i` at the Merchant prompt, at the Shrine prompt and at the dragon-encounter prompt each shows the Inventory, then the same prompt again, and the game continues normally (leave, walk past, retreat).
  - The full word, with mixed case and surrounding spaces, works; at least one run in a translated language using that language's full word.
  - Each of the four prompts shows the hint.
  - `1` at the menu after character creation shows the Character Summary with a Leather Armor line.
- **Combat stats display (existing seam)**: the armor line names Leather Armor for a new player and Magical Armor after a purchase. The existing "without armor, no armor line" test is replaced, since the player can no longer have no armor.
- **Existing unit tests updated**: Merchant purchase tests that expect the player's armor to be missing after a refused or non-armor purchase now expect Leather Armor. Dragon armor tests keep using Magical Armor.
- **Messages (existing seam)**: the existing same-keys-as-English test covers the new keys in every language.
- **Not tested**: `i` during a combat round. That prompt's code doesn't change, and the player's own die can't be scripted, so a whole-game run can't reach it reliably.
- **Prior art**: the whole-game run in the North path and end-of-input tests, the combat stats display tests, and the Merchant tests.

## Out of Scope

- Size or weight limits for the Inventory, and any new items to carry (potions, loot, keys…). Limits should be designed with the first feature that adds items to carry.
- Checking the Inventory during combat rounds or from the menu after character creation.
- Giving Leather Armor a real effect, or rebalancing armor, weapons, prices or the dragon.
- Changing the Merchant's purchase message, or adding a "you discard your Leather Armor" line.
- Rewording "Your Magical Armor absorbs the blow!".
- Showing Strength, Agility or Health Points in the Inventory (they stay in the Character Summary).
- Localizing the full words accepted at the path and dragon prompts (`north`, `attack`, etc.).

## Further Notes

- Use the glossary terms (Inventory, Leather Armor, Magical Armor, Gold, Merchant, Shrine) consistently in code, messages and tests.
- ADR-0001 requires every language to be complete and to avoid accented characters; this spec follows it.
- Leather Armor was described as a placeholder: it exists so there is always one armor to show, and it may get a real effect in a later feature.
