using System.Text.Json;

public class Messages
{
    public string CurrentLanguage { get; set; }
    private Dictionary<string, string> _dictionary;
    private Dictionary<string, string> _raceMap;
    private Dictionary<string, string> _occupationMap;
    private Dictionary<string, string> _displayRaceMap;
    private Dictionary<string, string> _displayOccupationMap;
    private Dictionary<string, string> _displayWeaponMap;
    private Dictionary<string, string> _merchantChoiceMap;
    private Dictionary<string, ShrineChoice> _shrineChoiceMap;
    private HashSet<string> _inventoryCommands;

    public Messages()
    {
        CurrentLanguage = "English";
        _dictionary = new Dictionary<string, string>();
        _raceMap = new Dictionary<string, string>();
        _occupationMap = new Dictionary<string, string>();
        _displayRaceMap = new Dictionary<string, string>();
        _displayOccupationMap = new Dictionary<string, string>();
        _displayWeaponMap = new Dictionary<string, string>();
        _merchantChoiceMap = new Dictionary<string, string>();
        _shrineChoiceMap = new Dictionary<string, ShrineChoice>();
        _inventoryCommands = new HashSet<string>();
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
        foreach (JsonProperty entry in langSection.GetProperty("dictionary").EnumerateObject())
            _dictionary[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _raceMap = new Dictionary<string, string>();
        foreach (JsonProperty entry in langSection.GetProperty("raceMap").EnumerateObject())
            _raceMap[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _occupationMap = new Dictionary<string, string>();
        foreach (JsonProperty entry in langSection.GetProperty("occupationMap").EnumerateObject())
            _occupationMap[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _displayRaceMap = new Dictionary<string, string>();
        foreach (JsonProperty entry in langSection.GetProperty("displayRaceMap").EnumerateObject())
            _displayRaceMap[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _displayOccupationMap = new Dictionary<string, string>();
        foreach (JsonProperty entry in langSection.GetProperty("displayOccupationMap").EnumerateObject())
            _displayOccupationMap[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _displayWeaponMap = new Dictionary<string, string>();
        foreach (JsonProperty entry in langSection.GetProperty("displayWeaponMap").EnumerateObject())
            _displayWeaponMap[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _merchantChoiceMap = new Dictionary<string, string>();
        foreach (JsonProperty entry in langSection.GetProperty("merchantChoiceMap").EnumerateObject())
            _merchantChoiceMap[entry.Name] = entry.Value.GetString() ?? string.Empty;

        _shrineChoiceMap = new Dictionary<string, ShrineChoice>();
        foreach (JsonProperty entry in langSection.GetProperty("shrineChoiceMap").EnumerateObject())
            _shrineChoiceMap[entry.Name] = Enum.Parse<ShrineChoice>(entry.Value.GetString() ?? string.Empty, ignoreCase: true);

        _inventoryCommands = new HashSet<string>();
        foreach (JsonProperty entry in langSection.GetProperty("inventoryCommandMap").EnumerateObject())
            _inventoryCommands.Add(entry.Name);
    }

    public string GetMessage(string key)
    {
        return _dictionary.ContainsKey(key) ? _dictionary[key] : $"[{key}]";
    }

    public bool IsValidRace(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return _raceMap.ContainsKey(input.Trim().ToLower());
    }

    public string NormalizeRace(string input)
    {
        return _raceMap.TryGetValue(input.Trim().ToLower(), out string? canonical) ? canonical : input;
    }

    public bool IsValidOccupation(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return _occupationMap.ContainsKey(input.Trim().ToLower());
    }

    public string NormalizeOccupation(string input)
    {
        return _occupationMap.TryGetValue(input.Trim().ToLower(), out string? canonical) ? canonical : input;
    }

    public string TranslateRaceForDisplay(string race)
    {
        return _displayRaceMap.TryGetValue(race, out string? translated) ? translated : race;
    }

    public string TranslateOccupationForDisplay(string occupation)
    {
        return _displayOccupationMap.TryGetValue(occupation, out string? translated) ? translated : occupation;
    }

    public string TranslateWeaponForDisplay(string weaponType)
    {
        return _displayWeaponMap.TryGetValue(weaponType, out string? translated) ? translated : weaponType;
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
        if (string.IsNullOrWhiteSpace(input)) return null;
        return _merchantChoiceMap.TryGetValue(input.Trim().ToLower(), out string? canonical) ? canonical : null;
    }

    // Returns the Shrine choice for a Shrine prompt input in the current language, or null if invalid.
    public ShrineChoice? NormalizeShrineChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        return _shrineChoiceMap.TryGetValue(input.Trim().ToLower(), out ShrineChoice choice) ? choice : null;
    }

    // Whether the input asks to check the Inventory in the current language ("i" or the full word).
    public bool IsInventoryCommand(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return _inventoryCommands.Contains(input.Trim().ToLower());
    }


}
