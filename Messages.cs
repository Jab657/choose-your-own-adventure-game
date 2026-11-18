using System.Text.Json;

public class Messages
{
    public string CurrentLanguage { get; set; }
    private Dictionary<string, string> _dictionary;
    // Every map section of the current language (race, occupation, display and choice maps), by section name.
    private Dictionary<string, Dictionary<string, string>> _maps;

    public Messages()
    {
        CurrentLanguage = "English";
        _dictionary = new Dictionary<string, string>();
        _maps = new Dictionary<string, Dictionary<string, string>>();
    }

    public void SetCurrentLanguage(string language)
    {
        CurrentLanguage = language;
    }

    public void ReadDictionary()
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        string jsonText = File.ReadAllText(jsonPath);
        using JsonDocument doc = JsonDocument.Parse(jsonText);

        JsonElement langSection = doc.RootElement.GetProperty(CurrentLanguage);

        _dictionary = new Dictionary<string, string>();
        _maps = new Dictionary<string, Dictionary<string, string>>();
        foreach (JsonProperty section in langSection.EnumerateObject())
        {
            Dictionary<string, string> entries = ReadStringMap(section.Value);
            if (section.Name == "dictionary")
                _dictionary = entries;
            else
                _maps[section.Name] = entries;
        }
    }

    private static Dictionary<string, string> ReadStringMap(JsonElement section)
    {
        var map = new Dictionary<string, string>();
        foreach (JsonProperty entry in section.EnumerateObject())
            map[entry.Name] = entry.Value.GetString() ?? string.Empty;
        return map;
    }

    // The canonical English value of an answer in the named choice map of the current language,
    // ignoring case and surrounding spaces, or null if the answer isn't in the map.
    public string? LookupChoice(string mapName, string? answer)
    {
        if (!_maps.TryGetValue(mapName, out Dictionary<string, string>? map))
            throw new KeyNotFoundException($"Choice map '{mapName}' is not loaded for {CurrentLanguage}.");
        if (string.IsNullOrWhiteSpace(answer)) return null;
        return map.TryGetValue(answer.Trim().ToLower(), out string? canonical) ? canonical : null;
    }

    private string TranslateForDisplay(string mapName, string englishName)
    {
        return _maps.TryGetValue(mapName, out Dictionary<string, string>? map)
            && map.TryGetValue(englishName, out string? translated) ? translated : englishName;
    }

    public string GetMessage(string key)
    {
        return _dictionary.ContainsKey(key) ? _dictionary[key] : $"[{key}]";
    }

    public bool IsValidRace(string? input)
    {
        return LookupChoice("raceMap", input) != null;
    }

    public string NormalizeRace(string input)
    {
        return LookupChoice("raceMap", input) ?? input;
    }

    public bool IsValidOccupation(string? input)
    {
        return LookupChoice("occupationMap", input) != null;
    }

    public string NormalizeOccupation(string input)
    {
        return LookupChoice("occupationMap", input) ?? input;
    }

    public string TranslateRaceForDisplay(string race)
    {
        return TranslateForDisplay("displayRaceMap", race);
    }

    public string TranslateOccupationForDisplay(string occupation)
    {
        return TranslateForDisplay("displayOccupationMap", occupation);
    }

    public string TranslateWeaponForDisplay(string weaponType)
    {
        return TranslateForDisplay("displayWeaponMap", weaponType);
    }

    // The armor's translated name and absorption, e.g. "Leather Armor (absorbs 0 damage)".
    public string TranslateArmorForDisplay(Armor armor)
    {
        string key = armor.Kind switch
        {
            ArmorKind.Leather => "leather_armor_display",
            ArmorKind.Magical => "magical_armor_display",
            _ => throw new ArgumentOutOfRangeException(nameof(armor), armor.Kind, "Unknown armor kind.")
        };
        return string.Format(GetMessage(key), armor.Absorption);
    }

    // Returns "armor", "weapon" or "leave" for a Merchant menu input in the current language, or null if invalid.
    public string? NormalizeMerchantChoice(string? input)
    {
        return LookupChoice("merchantChoiceMap", input);
    }

    // Returns the Shrine choice for a Shrine prompt input in the current language, or null if invalid.
    public ShrineChoice? NormalizeShrineChoice(string? input)
    {
        string? canonical = LookupChoice("shrineChoiceMap", input);
        return canonical == null ? null : Enum.Parse<ShrineChoice>(canonical, ignoreCase: true);
    }

    // Whether the input asks to check the Inventory in the current language ("i" or the full word).
    public bool IsInventoryCommand(string? input)
    {
        return LookupChoice("inventoryCommandMap", input) != null;
    }
}
