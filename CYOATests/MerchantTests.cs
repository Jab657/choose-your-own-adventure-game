// CYOATests/MerchantTests.cs
[TestClass]
[DoNotParallelize]
public class MerchantTests
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
    public void Visit_FirstVisit_ShowsOswinsOfferAndPlayersGold()
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        Messages messages = CreateMessages("English");

        string output = RunWithConsole("l\n", () => merchant.Visit(player, messages));

        StringAssert.Contains(output, "Oswin");
        StringAssert.Contains(output, "Magical Armor - 15 Gold");
        StringAssert.Contains(output, "Magical Weapon - 20 Gold");
        StringAssert.Contains(output, "You have 20 Gold.");
    }

    [TestMethod]
    [DataRow("l")]
    [DataRow("leave")]
    public void Visit_Leave_BuysNothingAndMarksMerchantAsTraded(string input)
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        Weapon weaponBefore = player.Weapon!;
        Messages messages = CreateMessages("English");

        string output = RunWithConsole(input + "\n", () => merchant.Visit(player, messages));

        Assert.IsTrue(merchant.HasTraded);
        Assert.AreEqual(20, player.Gold);
        Assert.AreSame(weaponBefore, player.Weapon);
        Assert.AreEqual(12, player.Weapon!.MaxDamage);
        StringAssert.Contains(output, messages.GetMessage("merchant_leave"));
    }

    [TestMethod]
    [DataRow("Spanish", "salir")]
    [DataRow("French", "partir")]
    [DataRow("Italian", "lasciare")]
    public void Visit_LeaveWithTranslatedFullWord_EndsVisit(string language, string leaveWord)
    {
        Merchant merchant = new Merchant();
        Messages messages = CreateMessages(language);

        string output = RunWithConsole(leaveWord + "\n", () => merchant.Visit(CreatePlayer(), messages));

        Assert.IsTrue(merchant.HasTraded);
        StringAssert.Contains(output, "Oswin");
        StringAssert.Contains(output, messages.GetMessage("merchant_leave"));
    }

    [TestMethod]
    [DataRow("  L  ")]
    [DataRow("LEAVE")]
    [DataRow(" Leave\t")]
    public void Visit_LeaveIgnoresCaseAndSurroundingWhitespace(string input)
    {
        Merchant merchant = new Merchant();
        Messages messages = CreateMessages("English");

        string output = RunWithConsole(input + "\n", () => merchant.Visit(CreatePlayer(), messages));

        Assert.IsTrue(merchant.HasTraded);
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
    }

    [TestMethod]
    public void Visit_InvalidInput_ShowsInvalidMessageAndPromptsAgain()
    {
        Merchant merchant = new Merchant();
        Messages messages = CreateMessages("English");

        string output = RunWithConsole("x\n\nl\n", () => merchant.Visit(CreatePlayer(), messages));

        string invalid = messages.GetMessage("invalid");
        int invalidCount = output.Split(invalid).Length - 1;
        int promptCount = output.Split(messages.GetMessage("merchant_prompt")).Length - 1;
        Assert.AreEqual(2, invalidCount);
        Assert.AreEqual(3, promptCount);
        Assert.IsTrue(merchant.HasTraded);
    }

    [TestMethod]
    [Timeout(5000)]
    public void Visit_SecondVisit_ShowsEmptyStallAndReturnsWithoutReadingInput()
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        Messages messages = CreateMessages("English");
        RunWithConsole("l\n", () => merchant.Visit(player, messages));

        // No input is scripted: if the second visit prompted, ReadLine would return null and loop forever.
        string output = RunWithConsole("", () => merchant.Visit(player, messages));

        StringAssert.Contains(output, messages.GetMessage("merchant_empty_stall"));
        Assert.IsFalse(output.Contains("Oswin, a Merchant"));
        Assert.IsFalse(output.Contains(messages.GetMessage("merchant_prompt")));
        Assert.AreEqual(20, player.Gold);
    }
}
