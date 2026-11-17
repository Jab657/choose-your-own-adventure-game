# Spec: Shrine Detour on the North path

Status: ready-for-agent

## Problem Statement

When a player takes the North path, the game says they walked for hours and then drops them straight in front of the dragon. Nothing happens on the way, so the trip North feels like a menu option rather than a journey, and the only choice the player makes there is whether to fight. Players who skipped the Merchant, or spent all their Gold there, have nothing else that affects how ready they are for the fight.

## Solution

On the way North, before reaching the dragon, the player always comes to a **Detour**: an old mountain **Shrine**. They make one choice:

- **Make an offering**: costs 5 Gold and always restores 5 Health Points.
- **Pray**: free, but a gamble. On a d6, 1–2 means the spirits are displeased and the player loses 2 Health Points (never below 1); 3–6 means they are pleased and the player gains 2 Health Points.
- **Walk past**: nothing changes.

After the outcome the player sees their new Health Points (and their new Gold after an offering). They then continue up the mountain and meet the dragon exactly as before. The Shrine never ends the game.

All new text is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want something to happen on the way North, so that the journey feels like part of the story and not just a menu option.
2. As a player, I want to come across the Shrine every time I go North, so that the Detour is a dependable part of the North path.
3. As a player, I want the Shrine to come before the dragon, so that what I choose there can help or hurt me in the fight.
4. As a player, I want the North path text to describe the journey, then the Shrine, then reaching the dragon, so that the story reads in a sensible order.
5. As a player, I want the Shrine to have its own atmospheric description, so that it feels like a real place in the cold mountains.
6. As a player, I want the Shrine to list my three options and what to type for each, so that I know what I can do.
7. As a player, I want to make an offering of Gold at the Shrine, so that I can turn spare Gold into health before the fight.
8. As a player, I want the offering to always restore the same amount of health, so that paying Gold is a safe, predictable choice.
9. As a player, I want the offering to cost Gold, so that the choice involves a real trade-off.
10. As a player who spent all my Gold at the Merchant, I want to be told I can't afford the offering, so that I understand why nothing happened.
11. As a player who can't afford the offering, I want to be shown how much Gold I have and what it costs, so that the refusal is clear.
12. As a player who can't afford the offering, I want to be asked again, so that I can still pray or walk past.
13. As a player with exactly enough Gold, I want the offering to work, so that the price is honest.
14. As a player, I want to pray at the Shrine for free, so that I have an option even with no Gold.
15. As a player, I want prayer to be a gamble that can help or hurt, so that praying is a risk rather than a free bonus.
16. As a player whose prayer is answered, I want to gain health, so that the gamble can pay off.
17. As a player whose prayer displeases the spirits, I want to lose some health, so that the risk is real.
18. As a player with very low health, I want a bad prayer to never take me below 1 Health Point, so that the Shrine itself can never kill me.
19. As a player, I want to walk past the Shrine without anything changing, so that I'm never forced to take a risk or spend Gold.
20. As a player, I want healing at the Shrine to be able to take me above my starting health, so that the offering is always worth something.
21. As a player, I want to see my new Health Points after the offering or prayer, so that I know how ready I am for the dragon.
22. As a player who made an offering, I want to see my remaining Gold, so that the cost feels real.
23. As a player, I want to carry on to the dragon after the Shrine whatever I chose, so that the Shrine is a step on the journey and not a different ending.
24. As a player, I want the dragon encounter to work exactly as before once I reach it, so that the Shrine doesn't change the fight's rules.
25. As a player, I want to choose with short codes or full words (`o`/offer, `p`/pray, `l`/leave), so that the Shrine works like the Merchant and dragon prompts I already know.
26. As a player, I want the full words to work in my own language, so that I can play fully in Spanish, French or Italian.
27. As a player, I want my input accepted regardless of capitals or surrounding spaces, so that small typing differences don't trip me up.
28. As a player, I want an invalid choice to tell me so and ask again, so that a typo doesn't skip the Shrine.
29. As a player whose input ends at the Shrine prompt, I want the game to stop cleanly, so that it doesn't loop forever.
30. As a player in any language, I want never to see raw placeholder text like `[shrine_intro]`, so that the game feels finished.
31. As a player in Spanish, French or Italian, I want the Shrine text to follow the same style as the rest of the game (no accented characters), so that it displays correctly in my console.
32. As a developer, I want the Shrine's rules testable without the console, so that edge cases like the 1 Health Point floor can be checked directly.
33. As a developer, I want the prayer roll to be controllable in tests, so that both outcomes and the boundary between them can be checked exactly.
34. As a developer, I want the existing key-coverage test to fail if any language is missing a Shrine key, so that translation gaps are caught before release.
35. As a developer, I want a full-game test of the North path order, so that a future change can't silently skip the Shrine or put it after the dragon.

## Implementation Decisions

- **Domain terms**: **Detour** (a short story scene always met on the North path before the dragon) and **Shrine** (the only Detour) are defined in the glossary. Only the Shrine is built; there is no general Detour framework that picks between Detours. One can be added once a second Detour exists.
- **Shrine module (new)**: a deep module modelled on the Merchant, owning all Shrine rules. It receives a die when it is created. Its interface:
  - A choose operation taking the player and a Shrine choice (Offer, Pray, Leave) and returning a result that says what happened: offering made, not enough Gold, prayer answered, prayer displeased, or walked past. It only changes player state; it never writes to the console. The player's Gold and Health Points change only as described below.
  - A visit operation taking the player and messages that runs the console scene: the Shrine description and options, then a loop on input until an offering, prayer or leave succeeds; then the outcome text and updated stats.
- **Rules**:
  - Offer: costs 5 Gold and gives +5 Health Points. If the player has less than 5 Gold, nothing changes and the result is not enough Gold.
  - Pray: rolls a d6 on the Shrine's die. 1–2 gives −2 Health Points, but Health Points never drop below 1 because of the Shrine. 3–6 gives +2 Health Points.
  - Leave: nothing changes.
  - Healing has no upper cap (the game has no maximum Health Points).
- **Console flow**: invalid input prints the existing invalid-choice message and asks again. Not enough Gold prints a message showing the cost and the player's current Gold, then asks again. After an offering, the new Health Points and Gold are printed; after a prayer, the new Health Points. Walking past prints a short leaving line. Input ending at the prompt follows the existing end-of-input behaviour: the game stops cleanly.
- **Input**: `o`/offer, `p`/pray, `l`/leave, case-insensitive and trimmed, with the full words translated per language through a new per-language Shrine choice map loaded by messages, like the existing Merchant choice map.
- **Game loop**: the North branch now runs: journey text → Shrine visit → dragon arrival text → existing dragon introduction, stats and prompt. One Shrine is created per North trip, using the game's die. Because the game ends after the dragon, the Shrine can only be visited once per game.
- **North narrative split**: the existing North narrative message is reworded to describe only the journey (walking for hours, colder air, sparser trees) and stops before the dragon. A new dragon-arrival message carries the "you press on… you are confronted with a dragon, with a hoard of gold behind it" text. This is done in all four languages.
- **Translations**: every new message key (Shrine description, options prompt, offering outcome, not enough Gold, prayer answered, prayer displeased, walk past, updated Health Points and Gold lines, dragon arrival) and the new Shrine choice map are added to English, Spanish, French and Italian, with no accented characters, per ADR-0001.

## Testing Decisions

- **What makes a good test**: assert on external behaviour (return values, the player's Gold and Health Points, printed messages, order of printed text) through public interfaces, not on private methods or internal structure. Tests should keep passing if the internals are refactored.
- **Shrine choose operation (new seam, primary)**, with a scripted die:
  - Offer with plenty of Gold: Gold −5, Health Points +5.
  - Offer with exactly 5 Gold: succeeds and leaves 0 Gold.
  - Offer with less than 5 Gold (e.g. 0 after buying the Magical Weapon): not-enough-Gold result, player unchanged.
  - Pray with rolls 1 and 2: displeased, Health Points −2. With rolls 3 and 6: answered, Health Points +2.
  - Bad prayer at 1 and 2 Health Points: player ends at 1.
  - Leave: player unchanged.
- **Shrine visit operation (new seam)**, with scripted console input and a scripted die:
  - Each short code and full word, plus at least one translated full word.
  - Case and whitespace tolerance.
  - Invalid input, then a valid choice.
  - Unaffordable offering, then walking past, which prints the not-enough-Gold message and ends with the player unchanged.
  - Updated Health Points (and Gold) are printed after an outcome.
  - Input ending at the Shrine prompt stops the game cleanly (no endless loop).
- **Whole game (existing seam)**: one scripted run through the game that goes North, walks past the Shrine, and asserts the order of text: journey → Shrine → dragon arrival → dragon prompt. Walking past keeps it deterministic, since the game creates its own die.
- **Messages (existing seam)**: the existing same-keys-as-English test covers the new keys in every language; add one test that the Italian Shrine choice map turns the Italian full words into the right choices.
- **Prior art**: the Merchant tests (choose operation with object initialisers, visit with redirected console input and output), the scripted die used in the dragon and combat tests, and the whole-game run in the end-of-input tests.

## Out of Scope

- A general Detour framework, more Detours, or picking a Detour at random.
- Detours on the South path.
- A Detour that ends the game or skips the dragon.
- Changing Strength, Agility, weapon or armor at the Shrine.
- A maximum Health Points cap.
- Shrine ASCII art.
- Injecting a die into the game so the whole-game test can script prayer rolls.
- Rebalancing the dragon or the Merchant prices.

## Further Notes

- Use the glossary terms (Detour, Shrine, Gold, Merchant) consistently in code, messages and tests.
- ADR-0001 requires every language to be complete and to avoid accented characters; this spec follows it.
- The Shrine is the first thing that changes Health Points outside combat. Health Points shown in the dragon fight's combat stats will reflect the Shrine outcome.
