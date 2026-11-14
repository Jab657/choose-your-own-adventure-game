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
    public void Purchase_MagicalArmor_TakesFifteenGoldAndGivesArmorThatAbsorbsThree()
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();

        PurchaseResult result = merchant.Purchase(player, MerchantItem.MagicalArmor);

        Assert.AreEqual(PurchaseResult.Bought, result);
        Assert.AreEqual(5, player.Gold);
        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(3, player.Armor.Absorption);
        Assert.IsTrue(merchant.HasTraded);
    }

    [TestMethod]
    public void Purchase_MagicalArmorWithLessThanFifteenGold_IsRefusedAndChangesNothing()
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        player.Gold = 14;

        PurchaseResult result = merchant.Purchase(player, MerchantItem.MagicalArmor);

        Assert.AreEqual(PurchaseResult.NotEnoughGold, result);
        Assert.AreEqual(14, player.Gold);
        Assert.IsNull(player.Armor);
        Assert.IsFalse(merchant.HasTraded);
    }

    [TestMethod]
    public void Purchase_AfterAPurchase_IsRefusedAsAlreadyTraded()
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        player.Gold = 40;
        merchant.Purchase(player, MerchantItem.MagicalArmor);
        Armor firstArmor = player.Armor!;

        PurchaseResult result = merchant.Purchase(player, MerchantItem.MagicalArmor);

        Assert.AreEqual(PurchaseResult.AlreadyTraded, result);
        Assert.AreEqual(25, player.Gold);
        Assert.AreSame(firstArmor, player.Armor);
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
    [DataRow("English", "a")]
    [DataRow("English", "armor")]
    [DataRow("English", " ARMOR ")]
    [DataRow("Spanish", "armadura")]
    [DataRow("French", "armure")]
    [DataRow("Italian", "armatura")]
    public void Visit_ChooseArmor_BuysMagicalArmorAndEndsVisit(string language, string input)
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        Messages messages = CreateMessages(language);

        string output = RunWithConsole(input + "\n", () => merchant.Visit(player, messages));

        Assert.AreEqual(5, player.Gold);
        Assert.IsNotNull(player.Armor);
        Assert.IsTrue(merchant.HasTraded);
        Assert.AreNotEqual("[merchant_bought_armor]", messages.GetMessage("merchant_bought_armor"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("merchant_bought_armor"), 15));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void Visit_ChooseArmorWithoutEnoughGold_ShowsRefusalAndPromptsAgain(string language)
    {
        Merchant merchant = new Merchant();
        Player player = CreatePlayer();
        player.Gold = 10;
        Messages messages = CreateMessages(language);

        string output = RunWithConsole("a\nl\n", () => merchant.Visit(player, messages));

        string refusal = messages.GetMessage("merchant_not_enough_gold");
        Assert.AreNotEqual("[merchant_not_enough_gold]", refusal);
        StringAssert.Contains(output, string.Format(refusal, 15, 10));
        int promptCount = output.Split(messages.GetMessage("merchant_prompt")).Length - 1;
        Assert.AreEqual(2, promptCount);
        Assert.IsFalse(output.Contains(messages.GetMessage("invalid")));
        Assert.AreEqual(10, player.Gold);
        Assert.IsNull(player.Armor);
        StringAssert.Contains(output, messages.GetMessage("merchant_leave"));
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
