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
    [DataRow("English", "o")]
    [DataRow("English", "offer")]
    [DataRow("English", "  O  ")]
    [DataRow("English", " Offer\t")]
    [DataRow("Spanish", "ofrecer")]
    [DataRow("French", "OFFRIR")]
    [DataRow("Italian", "offrire")]
    public void Visit_Offer_MakesOfferingAndShowsNewHealthPointsAndGold(string language, string input)
    {
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();
        Messages messages = CreateMessages(language);

        string output = RunWithConsole(input + "\n", () => shrine.Visit(player, messages));

        foreach (string key in new[] { "shrine_offering_made", "shrine_health_now", "shrine_gold_now" })
            Assert.AreNotEqual($"[{key}]", messages.GetMessage(key), $"'{key}' is missing in {language}.");

        StringAssert.Contains(output, string.Format(messages.GetMessage("shrine_offering_made"), 5));
        StringAssert.Contains(output, string.Format(messages.GetMessage("shrine_health_now"), 25));
        StringAssert.Contains(output, string.Format(messages.GetMessage("shrine_gold_now"), 15));
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
        Assert.IsFalse(output.Contains(messages.GetMessage("shrine_leave")));
        Assert.AreEqual(25, player.HealthPoints);
        Assert.AreEqual(15, player.Gold);
    }

    [TestMethod]
    [DataRow("English", "p")]
    [DataRow("English", "pray")]
    [DataRow("English", "  P  ")]
    [DataRow("English", " Pray\t")]
    [DataRow("Spanish", "rezar")]
    [DataRow("French", "PRIER")]
    [DataRow("Italian", "pregare")]
    public void Visit_PrayAnswered_ShowsAnsweredOutcomeAndNewHealthPoints(string language, string input)
    {
        Shrine shrine = new Shrine(new ScriptedDie(4));
        Player player = CreatePlayer();
        Messages messages = CreateMessages(language);

        string output = RunWithConsole(input + "\n", () => shrine.Visit(player, messages));

        Assert.AreNotEqual("[shrine_prayer_answered]", messages.GetMessage("shrine_prayer_answered"), $"Missing in {language}.");
        StringAssert.Contains(output, messages.GetMessage("shrine_prayer_answered"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("shrine_health_now"), 22));
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
        Assert.IsFalse(output.Contains(messages.GetMessage("shrine_leave")));
        Assert.AreEqual(22, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    [DataRow("English", "p")]
    [DataRow("English", " PRAY ")]
    [DataRow("Spanish", "Rezar")]
    [DataRow("French", "prier")]
    [DataRow("Italian", "PREGARE")]
    public void Visit_PrayDispleased_ShowsDispleasedOutcomeAndNewHealthPoints(string language, string input)
    {
        Shrine shrine = new Shrine(new ScriptedDie(2));
        Player player = CreatePlayer();
        Messages messages = CreateMessages(language);

        string output = RunWithConsole(input + "\n", () => shrine.Visit(player, messages));

        Assert.AreNotEqual("[shrine_prayer_displeased]", messages.GetMessage("shrine_prayer_displeased"), $"Missing in {language}.");
        StringAssert.Contains(output, messages.GetMessage("shrine_prayer_displeased"));
        Assert.IsFalse(output.Contains(messages.GetMessage("shrine_prayer_answered")));
        StringAssert.Contains(output, string.Format(messages.GetMessage("shrine_health_now"), 18));
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
        Assert.AreEqual(18, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    public void Visit_EnglishPray_PrintsOutcomeThenHealthPoints()
    {
        string output = RunWithConsole("p\n",
            () => new Shrine(new ScriptedDie(6)).Visit(CreatePlayer(), CreateMessages("English")));

        int outcome = output.IndexOf("Your prayer is answered.");
        int health = output.IndexOf("You now have 22 Health Points.");
        Assert.IsTrue(outcome >= 0 && outcome < health, output);
    }

    [TestMethod]
    public void Visit_EnglishOffer_PrintsOutcomeThenHealthPointsThenGold()
    {
        Messages messages = CreateMessages("English");

        string output = RunWithConsole("o\n", () => new Shrine(new ScriptedDie()).Visit(CreatePlayer(), messages));

        int outcome = output.IndexOf("You lay 5 Gold at the foot of the Shrine.");
        int health = output.IndexOf("You now have 25 Health Points.");
        int gold = output.IndexOf("You have 15 Gold left.");
        Assert.IsTrue(outcome >= 0 && outcome < health && health < gold, output);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void Visit_UnaffordableOfferingThenLeave_RefusesAsksAgainAndWalksPastUnchanged(string language)
    {
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();
        player.Gold = 0; // spent on the Magical Weapon
        Messages messages = CreateMessages(language);

        string output = RunWithConsole("o\nl\n", () => shrine.Visit(player, messages));

        Assert.AreNotEqual("[shrine_not_enough_gold]", messages.GetMessage("shrine_not_enough_gold"), $"Missing in {language}.");
        StringAssert.Contains(output, string.Format(messages.GetMessage("shrine_not_enough_gold"), 5, 0));
        int promptCount = output.Split(messages.GetMessage("shrine_prompt")).Length - 1;
        Assert.AreEqual(2, promptCount);
        Assert.IsFalse(output.Contains(string.Format(messages.GetMessage("shrine_offering_made"), 5)));
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
        StringAssert.Contains(output, messages.GetMessage("shrine_leave"));
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(20, player.HealthPoints);
    }

    [TestMethod]
    public void Visit_EnglishUnaffordableOffering_ShowsCostAndCurrentGold()
    {
        Player player = CreatePlayer();
        player.Gold = 3;

        string output = RunWithConsole("o\nl\n",
            () => new Shrine(new ScriptedDie()).Visit(player, CreateMessages("English")));

        StringAssert.Contains(output, "The offering costs 5 Gold, and you have only 3.");
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

    [TestMethod]
    public void Choose_OfferWithEnoughGold_TakesFiveGoldAndRestoresFiveHealthPoints()
    {
        // No rolls are scripted: an offering must not roll the die.
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();

        ShrineResult result = shrine.Choose(player, ShrineChoice.Offer);

        Assert.AreEqual(ShrineResult.OfferingMade, result);
        Assert.AreEqual(15, player.Gold);
        // 20 + 5 = 25: above the starting Health Points, since healing has no upper cap.
        Assert.AreEqual(25, player.HealthPoints);
    }

    [TestMethod]
    public void Choose_OfferWithExactlyFiveGold_SucceedsAndLeavesNoGold()
    {
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();
        player.Gold = 5;

        ShrineResult result = shrine.Choose(player, ShrineChoice.Offer);

        Assert.AreEqual(ShrineResult.OfferingMade, result);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(25, player.HealthPoints);
    }

    [TestMethod]
    [DataRow(3)]
    [DataRow(6)]
    public void Choose_PrayWithRollOfThreeOrMore_AnswersPrayerAndAddsTwoHealthPoints(int roll)
    {
        Shrine shrine = new Shrine(new ScriptedDie(roll));
        Player player = CreatePlayer();

        ShrineResult result = shrine.Choose(player, ShrineChoice.Pray);

        Assert.AreEqual(ShrineResult.PrayerAnswered, result);
        // 20 + 2 = 22: above the starting Health Points, since healing has no upper cap.
        Assert.AreEqual(22, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Choose_PrayWithRollOfOneOrTwo_DispleasesSpiritsAndTakesTwoHealthPoints(int roll)
    {
        Shrine shrine = new Shrine(new ScriptedDie(roll));
        Player player = CreatePlayer();

        ShrineResult result = shrine.Choose(player, ShrineChoice.Pray);

        Assert.AreEqual(ShrineResult.PrayerDispleased, result);
        Assert.AreEqual(18, player.HealthPoints);
        Assert.AreEqual(20, player.Gold);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Choose_DispleasingPrayerAtLowHealthPoints_NeverDropsBelowOne(int healthPoints)
    {
        Shrine shrine = new Shrine(new ScriptedDie(1));
        Player player = CreatePlayer();
        player.HealthPoints = healthPoints;

        ShrineResult result = shrine.Choose(player, ShrineChoice.Pray);

        Assert.AreEqual(ShrineResult.PrayerDispleased, result);
        Assert.AreEqual(1, player.HealthPoints);
    }

    [TestMethod]
    [DataRow(0)] // after buying the Magical Weapon
    [DataRow(4)]
    public void Choose_OfferWithLessThanFiveGold_ReturnsNotEnoughGoldAndLeavesPlayerUnchanged(int gold)
    {
        Shrine shrine = new Shrine(new ScriptedDie());
        Player player = CreatePlayer();
        player.Gold = gold;

        ShrineResult result = shrine.Choose(player, ShrineChoice.Offer);

        Assert.AreEqual(ShrineResult.NotEnoughGold, result);
        Assert.AreEqual(gold, player.Gold);
        Assert.AreEqual(20, player.HealthPoints);
    }
}
