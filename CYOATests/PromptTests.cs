// CYOATests/PromptTests.cs
[TestClass]
[DoNotParallelize]
public class PromptTests
{
    private static Messages CreateMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Runs the action with scripted console input and returns everything written to the console.
    private static string RunWithConsole(string input, Action action)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        var output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader(input));
            Console.SetOut(output);
            action();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        return output.ToString();
    }

    [TestMethod]
    public void Ask_ValidAnswer_ReturnsCanonicalValue()
    {
        Prompt prompt = new Prompt(CreateMessages("English"));
        string? answer = null;

        string output = RunWithConsole("armor\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid"));

        Assert.AreEqual("armor", answer);
        Assert.AreEqual("Enter 'a' for armor, 'w' for weapon, or 'l' to leave." + Environment.NewLine, output);
    }

    [TestMethod]
    public void Ask_InvalidThenValid_PrintsInvalidMessageThenWholeQuestionAgain()
    {
        Prompt prompt = new Prompt(CreateMessages("English"));
        string? answer = null;

        // The path's invalid message, to show Ask prints whichever invalid message it is given.
        string output = RunWithConsole("x\n\nl\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "path_invalid"));

        string question = "Enter 'a' for armor, 'w' for weapon, or 'l' to leave." + Environment.NewLine;
        string invalid = "Invalid choice. Please enter 'n', 's', or 'e'." + Environment.NewLine;
        Assert.AreEqual(question + invalid + question + invalid + question, output);
        Assert.AreEqual("leave", answer);
    }

    private static Player CreatePlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 15,
            HealthPoints = 20,
            Gold = 20,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void Ask_InventoryOffered_ShowsHintThenInventoryThenWholeQuestionAgainLeavingPlayerUnchanged()
    {
        Prompt prompt = new Prompt(CreateMessages("English"));
        Player player = CreatePlayer();
        string? answer = null;

        string output = RunWithConsole("i\nl\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid",
            offerInventoryOf: player));

        string question = "Enter 'a' for armor, 'w' for weapon, or 'l' to leave." + Environment.NewLine
            + "Enter 'i' to check your inventory." + Environment.NewLine
            + "Enter choice: ";
        string inventory = string.Join(Environment.NewLine,
            "----- Inventory -----",
            "Weapon: long sword",
            "Weapon's Maximum Damage Points: 12",
            "Armor: Leather Armor (absorbs 0 damage)",
            "Gold: 20") + Environment.NewLine;
        Assert.AreEqual(question + inventory + question, output);
        Assert.AreEqual("leave", answer);
        Assert.AreEqual(20, player.Gold);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual("long sword", player.Weapon!.Type);
        Assert.AreEqual(ArmorKind.Leather, player.Armor.Kind);
    }

    [TestMethod]
    public void Ask_InventoryNotOffered_TreatsInventoryCommandAsInvalidAndPrintsNoHint()
    {
        Prompt prompt = new Prompt(CreateMessages("English"));
        string? answer = null;

        string output = RunWithConsole("i\nl\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid"));

        string question = "Enter 'a' for armor, 'w' for weapon, or 'l' to leave." + Environment.NewLine;
        Assert.AreEqual(question + "Invalid choice. Please try again." + Environment.NewLine + question, output);
        Assert.AreEqual("leave", answer);
    }

    [TestMethod]
    public void Ask_SameLine_PrintsEnterChoiceWithoutInventoryHint()
    {
        Prompt prompt = new Prompt(CreateMessages("English"));
        string? answer = null;

        string output = RunWithConsole("l\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid", sameLine: true));

        Assert.AreEqual("Enter 'a' for armor, 'w' for weapon, or 'l' to leave." + Environment.NewLine + "Enter choice: ", output);
        Assert.AreEqual("leave", answer);
    }

    [TestMethod]
    [DataRow("  ARMOR  ")]
    [DataRow("Armor\t")]
    [DataRow(" A ")]
    public void Ask_MixedCaseAndSurroundingSpaces_StillMatch(string input)
    {
        Prompt prompt = new Prompt(CreateMessages("English"));
        string? answer = null;

        string output = RunWithConsole(input + "\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid"));

        Assert.AreEqual("armor", answer);
        Assert.IsFalse(output.Contains("Invalid choice."));
    }

    [TestMethod]
    [Timeout(5000)]
    [DataRow("")]
    [DataRow("x\n")]
    public void Ask_InputEnds_ThrowsEndOfInput(string input)
    {
        Prompt prompt = new Prompt(CreateMessages("English"));

        Assert.ThrowsException<EndOfInputException>(() => RunWithConsole(input, () => prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid",
            offerInventoryOf: CreatePlayer())));
    }

    [TestMethod]
    [DataRow("Spanish", "raceMap", "Enano", "Dwarf")]
    [DataRow("Italian", "raceMap", "Nano", "Dwarf")]
    [DataRow("Italian", "occupationMap", "arciere", "Archer")]
    [DataRow("French", "merchantChoiceMap", "armure", "armor")]
    [DataRow("Italian", "shrineChoiceMap", "offrire", "offer")]
    [DataRow("Italian", "shrineChoiceMap", "pregare", "pray")]
    [DataRow("Italian", "shrineChoiceMap", "lasciare", "leave")]
    public void Ask_TranslatedAnswer_ReturnsEnglishCanonicalValue(string language, string choiceMap, string input, string expected)
    {
        Prompt prompt = new Prompt(CreateMessages(language));
        string? answer = null;

        RunWithConsole(input + "\n", () => answer = prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: choiceMap, invalidKey: "invalid"));

        Assert.AreEqual(expected, answer);
    }

    [TestMethod]
    public void Ask_EnglishWordInItalian_IsInvalid()
    {
        Messages messages = CreateMessages("Italian");
        Prompt prompt = new Prompt(messages);
        string? answer = null;

        string output = RunWithConsole("leave\nlasciare\n", () => answer = prompt.Ask(
            questionKey: "shrine_prompt", choiceMap: "shrineChoiceMap", invalidKey: "invalid"));

        StringAssert.Contains(output, messages.GetMessage("invalid"));
        Assert.AreEqual("leave", answer);
    }

    [TestMethod]
    [DataRow("Italian", "inventario", "----- Inventario -----")]
    [DataRow("Spanish", "INVENTARIO", "----- Inventario -----")]
    [DataRow("French", " inventaire ", "----- Inventaire -----")]
    public void Ask_TranslatedInventoryWord_ShowsInventory(string language, string input, string heading)
    {
        Messages messages = CreateMessages(language);
        Prompt prompt = new Prompt(messages);

        string output = RunWithConsole(input + "\nl\n", () => prompt.Ask(
            questionKey: "merchant_prompt", choiceMap: "merchantChoiceMap", invalidKey: "invalid",
            offerInventoryOf: CreatePlayer()));

        StringAssert.Contains(output, heading);
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
    }
}
