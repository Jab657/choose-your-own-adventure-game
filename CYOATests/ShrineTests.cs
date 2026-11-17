// CYOATests/ShrineTests.cs
[TestClass]
[DoNotParallelize]
public class ShrineTests
{
    private static Messages CreateMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
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
    [DataRow("English", "l")]
    [DataRow("English", "leave")]
    [DataRow("English", "  L  ")]
    [DataRow("English", " Leave\t")]
    [DataRow("Spanish", "salir")]
    [DataRow("French", "PARTIR")]
    [DataRow("Italian", "lasciare")]
    public void Visit_Leave_ShowsShrineThenWalksPastLeavingPlayerUnchanged(string language, string input)
    {
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();
        Messages messages = CreateMessages(language);

        string output = RunWithConsole(input + "\n", () => shrine.Visit(player, messages));

        foreach (string key in new[] { "shrine_intro", "shrine_prompt", "shrine_leave" })
        {
            Assert.AreNotEqual($"[{key}]", messages.GetMessage(key), $"'{key}' is missing in {language}.");
            StringAssert.Contains(output, messages.GetMessage(key));
        }
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    public void Visit_EnglishPrompt_ListsOfferPrayAndLeaveCodes()
    {
        Messages messages = CreateMessages("English");

        string output = RunWithConsole("l\n", () => new Shrine(new ScriptedDie()).Visit(CreatePlayer(), messages));

        StringAssert.Contains(output, "'o'");
        StringAssert.Contains(output, "'p'");
        StringAssert.Contains(output, "'l'");
    }

    [TestMethod]
    public void Visit_InvalidInput_ShowsInvalidMessageAndPromptsAgain()
    {
        Messages messages = CreateMessages("English");
        Player player = CreatePlayer();

        string output = RunWithConsole("x\n\nl\n", () => new Shrine(new ScriptedDie()).Visit(player, messages));

        int invalidCount = output.Split(messages.GetMessage("invalid")).Length - 1;
        int promptCount = output.Split(messages.GetMessage("shrine_prompt")).Length - 1;
        Assert.AreEqual(2, invalidCount);
        Assert.AreEqual(3, promptCount);
        StringAssert.Contains(output, messages.GetMessage("shrine_leave"));
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    [Timeout(5000)]
    public void Visit_InputEndsAtPrompt_StopsInsteadOfLooping()
    {
        Messages messages = CreateMessages("English");

        Assert.ThrowsException<EndOfInputException>(
            () => RunWithConsole("", () => new Shrine(new ScriptedDie()).Visit(CreatePlayer(), messages)));
    }

    [TestMethod]
    public void Choose_Leave_WalksPastAndLeavesPlayerUnchanged()
    {
        // No rolls are scripted: walking past must not roll the die.
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();

        ShrineResult result = shrine.Choose(player, ShrineChoice.Leave);

        Assert.AreEqual(ShrineResult.WalkedPast, result);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }
}
