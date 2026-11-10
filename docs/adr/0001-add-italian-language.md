# ADR 0001: Add Italian as a supported language

- **Status:** Accepted
- **Date:** 2026-11-09
- **Decider:** Jacob Colombo

## Context

The game supports English, Spanish and French. All player-facing text lives in
`language_data.json`, one section per language, and `Messages` loads the section
for the language picked at startup. `Game.cs` shows a numbered language menu and
maps the player's choice to a section name.

Each language section has six parts:

- `dictionary`: every message the game prints
- `raceMap` / `occupationMap`: what the player can type, mapped to the
  English race/occupation names the game logic uses
- `displayRaceMap` / `displayOccupationMap` / `displayWeaponMap`: how those
  English names are shown in the chosen language

The decision was whether to add Italian as a fourth language, and how.

## Decision

Add Italian as a fourth language, using the same approach as the existing
languages:

1. Add an `"Italian"` section to `language_data.json` with every key that the
   English section has.
2. Add `4. Italiano` to the language menu in `Game.cs` and map `"4"` to
   `"Italian"`.
3. Add tests in `CYOATests/MessagesTests.cs` covering Italian messages and
   inputs, including one that fails if any English key is missing from Italian.

Translation choices, made to match the Spanish and French sections:

- **No accented characters** ("e" for "è", "piu" for "più"), so text displays
  correctly in consoles that don't handle non-ASCII characters well.
- **Short win/lose/retreat endings**, like Spanish and French, rather than the
  longer English endings.
- **"Halfling" stays "Halfling"**, since Italian fantasy usage commonly keeps
  the English word.
- **Players still type `roll`** for stat rolls, because the game checks for
  that English word in every language.

## Alternatives considered

- **Keep three languages.** No work needed, but Italian-speaking players can't
  play in their language.
- **Move all languages to .NET resource files (`.resx`).** This is the standard
  .NET way to localize, but it would mean rewriting `Messages` and the existing
  three languages. Not worth it for one more language when the JSON approach
  works.
- **Use proper Italian accents.** More correct Italian, but inconsistent with
  the other languages and could print wrongly in some consoles. Can be revisited
  for all languages together.

## Consequences

**Positive**

- Italian speakers can play the whole game in Italian, including typing races
  and occupations in Italian.
- No code changes to `Messages`; the existing design supported a new language
  with data plus one menu line.
- The new key-coverage test catches missing Italian keys when new messages are
  added to English.

**Negative / follow-ups**

- Every new message now needs four translations instead of three.
- The language menu is a hard-coded switch in `Game.cs`; each new language means
  editing code as well as data. Generating the menu from the JSON sections would
  remove that.
- Some text is still English in every language: the dragon weapon art
  (`<<< claws >>>`) and the `roll` command.
- Italian text without accents is understandable but not fully correct spelling.
