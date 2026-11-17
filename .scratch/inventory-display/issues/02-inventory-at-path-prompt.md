# 02: Check the Inventory at the path prompt

**What to build:** At the path prompt (North / South / exit), the player can type `i`, or the word for "inventory" in their language, to check their Inventory: a translated `----- Inventory -----` heading, then their weapon, its maximum damage, their armor and their Gold. Checking is free: nothing changes, no invalid-choice message is printed, and the path prompt is shown again. A hint under the path prompt tells the player that `i` is available. This ticket builds the Inventory command and display end to end at one prompt; ticket 03 adds them to the other three. See `.scratch/inventory-display/spec.md`.

**Blocked by:** 01 (Start out wearing Leather Armor)

**Status:** ready-for-agent

- [ ] The player can display their Inventory. It prints the translated heading (styled like the Character Summary heading), then the weapon, its maximum damage, the armor (using the shared armor display from ticket 01) and the Gold, each with the Character Summary's existing labels. Gold always shows, including 0.
- [ ] Messages can tell whether an input is the Inventory command in the current language. Each language section gets a new Inventory command map listing `i` and the full word (`inventory` / `inventario` / `inventaire` / `inventario`), loaded like the Merchant and Shrine choice maps. Input is trimmed and compared without regard to case.
- [ ] The path prompt checks for the Inventory command first. On a match it displays the Inventory and shows the path prompt again.
- [ ] A new translated hint message ("Enter 'i' to check your inventory.") is printed after the path prompt line, before "Enter choice:". The path prompt message itself is not reworded.
- [ ] If input ends at the path prompt, the game still stops cleanly.
- [ ] All new keys (heading, hint) and the Inventory command map exist in all four languages with no accented characters (ADR-0001), and the existing same-keys-as-English test passes.
- [ ] Tests (whole-game runs on scripted input):
  - `i` at the path prompt for a new Fighter shows the heading, long sword, 12, Leather Armor (absorbs 0 damage) and 20 Gold, then the path prompt again.
  - South → buy Magical Armor → `i` shows Magical Armor (absorbs 3 damage) and 5 Gold.
  - South → buy the Magical Weapon → `i` shows the magical long sword, its higher damage and 0 Gold.
  - the full word with mixed case and surrounding spaces works.
  - a run in a translated language using that language's full word shows the translated heading.
  - the hint appears under the path prompt.
