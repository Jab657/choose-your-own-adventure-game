// CYOATests/MessagesTests.cs
[TestClass]
public class MessagesTests
{
    [TestMethod]
    public void Constructor_DefaultLanguageIsEnglish()
    {
        Messages messages = new Messages();
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void TranslateRaceForDisplay_BeforeReadDictionary_ReturnsInput()
    {
        Messages messages = new Messages();
        Assert.AreEqual("Elf", messages.TranslateRaceForDisplay("Elf"));
    }

    [TestMethod]
    public void TranslateOccupationForDisplay_BeforeReadDictionary_ReturnsInput()
    {
        Messages messages = new Messages();
        Assert.AreEqual("Fighter", messages.TranslateOccupationForDisplay("Fighter"));
    }

    [TestMethod]
    public void TranslateWeaponForDisplay_BeforeReadDictionary_ReturnsInput()
    {
        Messages messages = new Messages();
        Assert.AreEqual("dagger", messages.TranslateWeaponForDisplay("dagger"));
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        Assert.AreEqual("Spanish", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        Assert.AreEqual("French", messages.CurrentLanguage);
    }

    [TestMethod]
    public void SetCurrentLanguage_SetsLanguageToEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.SetCurrentLanguage("English");
        Assert.AreEqual("English", messages.CurrentLanguage);
    }

    [TestMethod]
    public void ReadDictionary_LoadsEnglishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Welcome to Choose Your Adventure Game", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsSpanishWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenido al Juego de Elige Tu Aventura", result);
    }

    [TestMethod]
    public void ReadDictionary_LoadsFrenchWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        string result = messages.GetMessage("welcome");
        Assert.AreEqual("Bienvenue dans Choisissez Votre Aventure", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsFallbackForUnknownKey()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string result = messages.GetMessage("unknown_key");
        Assert.AreEqual("[unknown_key]", result);
    }

    [TestMethod]
    public void GetMessage_ReturnsDifferentMessageAfterLanguageSwitch()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        string english = messages.GetMessage("welcome");

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        string spanish = messages.GetMessage("welcome");

        Assert.AreNotEqual(english, spanish);
    }

    [TestMethod]
    public void ReadDictionary_LoadsGoodbyeMessageInEachLanguage()
    {
        Messages messages = new Messages();

        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Adventure ends. Bye.", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();
        Assert.AreEqual("Adios!", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();
        Assert.AreEqual("Au revoir!", messages.GetMessage("goodbye"));

        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        Assert.AreEqual("Arrivederci!", messages.GetMessage("goodbye"));
    }

    [TestMethod]
    public void ReadDictionary_LoadsItalianWelcomeMessage()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        Assert.AreEqual("Benvenuto in Scegli la Tua Avventura", messages.GetMessage("welcome"));
    }

    [TestMethod]
    public void ReadDictionary_EveryLanguageHasEveryEnglishKey()
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));
        var englishKeys = doc.RootElement.GetProperty("English").GetProperty("dictionary").EnumerateObject().Select(p => p.Name).ToList();
        var languages = doc.RootElement.EnumerateObject().Select(p => p.Name).Where(name => name != "English").ToList();

        Assert.IsTrue(languages.Count > 0, "No languages other than English were found.");

        foreach (string language in languages)
        {
            Messages messages = new Messages();
            messages.SetCurrentLanguage(language);
            messages.ReadDictionary();

            foreach (string key in englishKeys)
                Assert.AreNotEqual($"[{key}]", messages.GetMessage(key), $"Key '{key}' is missing from {language} dictionary.");
        }
    }

    [TestMethod]
    [DataRow("English", "magical long sword", "magical long sword")]
    [DataRow("English", "magical lightning bolt spell", "magical lightning bolt spell")]
    [DataRow("English", "magical dagger", "magical dagger")]
    [DataRow("English", "magical long bow", "magical long bow")]
    [DataRow("Spanish", "magical long sword", "espada larga magica")]
    [DataRow("Spanish", "magical lightning bolt spell", "hechizo magico de rayo")]
    [DataRow("Spanish", "magical dagger", "daga magica")]
    [DataRow("Spanish", "magical long bow", "arco largo magico")]
    [DataRow("French", "magical long sword", "epee longue magique")]
    [DataRow("French", "magical lightning bolt spell", "sort magique d'eclair")]
    [DataRow("French", "magical dagger", "dague magique")]
    [DataRow("French", "magical long bow", "arc long magique")]
    [DataRow("Italian", "magical long sword", "spada lunga magica")]
    [DataRow("Italian", "magical lightning bolt spell", "incantesimo magico del fulmine")]
    [DataRow("Italian", "magical dagger", "pugnale magico")]
    [DataRow("Italian", "magical long bow", "arco lungo magico")]
    public void TranslateWeaponForDisplay_MagicalWeapon_HasFullNameInEachLanguage(string language, string magicalType, string expected)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        Assert.AreEqual(expected, messages.TranslateWeaponForDisplay(magicalType));
    }

    [TestMethod]
    public void ReadDictionary_EveryLanguageNamesEveryMagicalWeapon()
    {
        string[] magicalTypes = { "magical long sword", "magical lightning bolt spell", "magical dagger", "magical long bow" };
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));

        foreach (System.Text.Json.JsonProperty language in doc.RootElement.EnumerateObject())
        {
            System.Text.Json.JsonElement weaponMap = language.Value.GetProperty("displayWeaponMap");
            foreach (string magicalType in magicalTypes)
            {
                Assert.IsTrue(weaponMap.TryGetProperty(magicalType, out System.Text.Json.JsonElement name),
                    $"'{magicalType}' is missing from the {language.Name} weapon display map.");
                string? value = name.GetString();
                Assert.IsFalse(string.IsNullOrWhiteSpace(value), $"'{magicalType}' is empty in {language.Name}.");
                Assert.IsFalse(value!.Contains('{'), $"'{magicalType}' in {language.Name} is a placeholder pattern: '{value}'.");
            }
        }
    }

    [TestMethod]
    public void ItalianRaceAndOccupationInputsNormalizeToCanonical()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        Assert.AreEqual("Dwarf", messages.NormalizeRace("Nano"));
        Assert.AreEqual("Archer", messages.NormalizeOccupation("arciere"));
        Assert.AreEqual("spada lunga", messages.TranslateWeaponForDisplay("long sword"));
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInEnglish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from English dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInSpanish()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Spanish");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from Spanish dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_AllRequiredKeysExistInFrench()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("French");
        messages.ReadDictionary();

        string[] requiredKeys = { "welcome", "menu",
                                   "choose_path", "path_north", "path_south", "attack_prompt",
                                   "victory", "defeat", "retreat", "invalid", "goodbye" };

        foreach (string key in requiredKeys)
        {
            string result = messages.GetMessage(key);
            Assert.AreNotEqual($"[{key}]", result, $"Key '{key}' is missing from French dictionary.");
        }
    }

    // --- IsValidRace ---

    [TestMethod]
    public void IsValidRace_ReturnsTrue_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("human"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidRace("HUMAN"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForInvalidRace()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace("orc"));
    }

    [TestMethod]
    public void IsValidRace_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidRace(null));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_ForHuman()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("human"));
    }

    [TestMethod]
    public void NormalizeRace_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Human", messages.NormalizeRace("HUMAN"));
    }

    // --- IsValidOccupation ---

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("fighter"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsTrue_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsTrue(messages.IsValidOccupation("FIGHTER"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForInvalidOccupation()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation("wizard"));
    }

    [TestMethod]
    public void IsValidOccupation_ReturnsFalse_ForNull()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.IsFalse(messages.IsValidOccupation(null));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_ForFighter()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("fighter"));
    }

    [TestMethod]
    public void NormalizeOccupation_ReturnsCanonical_CaseInsensitive()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        Assert.AreEqual("Fighter", messages.NormalizeOccupation("FIGHTER"));
    }
}
