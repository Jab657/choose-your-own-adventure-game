[TestClass]
public class PlayerTests
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
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    [TestMethod]
    [DoNotParallelize]
    public void CreateCharacter_StartsWithTwentyGold()
    {
        Messages messages = CreateMessages("English");
        Player player = new Player();
        // Race, name, occupation, three "roll" prompts (strength, agility, health), then "2" to continue.
        string input = string.Join(Environment.NewLine, "human", "Aria", "fighter", "roll", "roll", "roll", "2") + Environment.NewLine;

        RunWithConsole(input, () => player.CreateCharacter(messages));

        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow("English", "Gold: 20")]
    [DataRow("Spanish", "Oro: 20")]
    [DataRow("French", "Or: 20")]
    [DataRow("Italian", "Oro: 20")]
    public void DisplayStats_ShowsGoldLine(string language, string expectedLine)
    {
        Messages messages = CreateMessages(language);
        Player player = new Player { Name = "Aria", Race = "Human", Occupation = "Fighter", Gold = 20 };

        string output = RunWithConsole("", () => player.DisplayStats(messages));

        string[] lines = output.Split(Environment.NewLine);
        CollectionAssert.Contains(lines, expectedLine);
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow("English", "Armor: Magical Armor (absorbs 3 damage)")]
    [DataRow("Spanish", "Armadura: Armadura Magica (absorbe 3 de dano)")]
    [DataRow("French", "Armure: Armure Magique (absorbe 3 degats)")]
    [DataRow("Italian", "Armatura: Armatura Magica (assorbe 3 danni)")]
    public void DisplayStats_WithArmor_ShowsArmorLine(string language, string expectedLine)
    {
        Messages messages = CreateMessages(language);
        Player player = new Player { Name = "Aria", Race = "Human", Occupation = "Fighter", Armor = Armor.MagicalArmor() };

        string output = RunWithConsole("", () => player.DisplayStats(messages));

        string[] lines = output.Split(Environment.NewLine);
        CollectionAssert.Contains(lines, expectedLine);
    }

    [TestMethod]
    [DoNotParallelize]
    public void DisplayStats_WithoutArmor_ShowsNoArmorLine()
    {
        Messages messages = CreateMessages("English");
        Player player = new Player { Name = "Aria", Race = "Human", Occupation = "Fighter" };

        string output = RunWithConsole("", () => player.DisplayStats(messages));

        Assert.IsFalse(output.Contains(messages.GetMessage("stats_armor")));
    }
}
