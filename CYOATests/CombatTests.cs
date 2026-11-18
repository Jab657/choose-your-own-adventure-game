[TestClass]
[DoNotParallelize]
public class CombatTests
{
    private const string AttackPrompt = "Enter 'a' to attack or 'r' to retreat.";
    private const string InvalidMessage = "Invalid choice. Please try again.";
    private const string DragonAttackLine = "Smolderfang attacks TestHero with claws!";

    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    private static Combat CreateCombat(Player player, Dragon dragon, Messages messages)
    {
        return new Combat(player, dragon, messages, new Prompt(messages));
    }

    // Health high enough that no attack round can end the fight, so only the answers decide it.
    private Combat CreateCombatNobodyCanWin(Messages messages)
    {
        Player player = CreateTestPlayer();
        player.HealthPoints = 1000;
        Dragon dragon = CreateTestDragon();
        dragon.HealthPoints = 1000;
        return CreateCombat(player, dragon, messages);
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

    // Each attack round ends with the dragon's attack, which opens with this line.
    private static int CountAttackRounds(string output)
    {
        return CountLines(output, DragonAttackLine);
    }

    private static int CountLines(string output, string line)
    {
        return output.Split(Environment.NewLine).Count(outputLine => outputLine == line);
    }

    [TestMethod]
    public void StartCombat_InvalidAnswerThenRetreat_AsksAgainWithoutAnotherAttackRound()
    {
        Combat combat = CreateCombatNobodyCanWin(CreateEnglishMessages());

        string output = RunWithConsole("x\nr\n", () => combat.StartCombat());

        Assert.AreEqual(1, CountAttackRounds(output));
        Assert.AreEqual(1, CountLines(output, InvalidMessage));
        Assert.AreEqual(2, CountLines(output, AttackPrompt));
        Assert.IsTrue(output.IndexOf(InvalidMessage) < output.LastIndexOf(AttackPrompt),
            "The prompt should be shown again after the invalid message.");
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    public void StartCombat_InventoryCommand_IsInvalidAndDoesNotShowInventory()
    {
        Messages messages = CreateEnglishMessages();
        Combat combat = CreateCombatNobodyCanWin(messages);

        string output = RunWithConsole("i\nr\n", () => combat.StartCombat());

        Assert.AreEqual(1, CountAttackRounds(output));
        Assert.AreEqual(1, CountLines(output, InvalidMessage));
        Assert.AreEqual(2, CountLines(output, AttackPrompt));
        Assert.IsFalse(output.Contains(messages.GetMessage("inventory_header")), "Combat should not show the Inventory.");
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    public void StartCombat_AttackThenRetreat_RunsAnotherAttackRoundThenRetreats()
    {
        Combat combat = CreateCombatNobodyCanWin(CreateEnglishMessages());

        string output = RunWithConsole(" A \nretreat\n", () => combat.StartCombat());

        Assert.AreEqual(2, CountAttackRounds(output));
        Assert.AreEqual(0, CountLines(output, InvalidMessage));
        Assert.AreEqual(1, CountLines(output, "You hastily retreat licking your wounds."));
        Assert.IsTrue(combat.PlayerRetreated);
    }

    [TestMethod]
    [DataRow("", DisplayName = "at the first prompt")]
    [DataRow("x\n", DisplayName = "after an invalid answer")]
    public void StartCombat_InputEndsAtPrompt_ThrowsEndOfInput(string input)
    {
        Combat combat = CreateCombatNobodyCanWin(CreateEnglishMessages());

        Assert.ThrowsException<EndOfInputException>(() => RunWithConsole(input, () => combat.StartCombat()));
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_DoesNotShowGold()
    {
        Player player = CreateTestPlayer();
        player.Gold = 20;
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(result.Contains(messages.GetMessage("stats_gold")), "Combat stats should not show Gold.");
    }

    [TestMethod]
    public void GetCombatStatsDisplay_AfterBuyingMagicalArmor_ShowsMagicalArmorLine()
    {
        Player player = CreateTestPlayer();
        player.Gold = 20;
        new Merchant().Purchase(player, MerchantItem.MagicalArmor);
        Combat combat = CreateCombat(player, CreateTestDragon(), CreateEnglishMessages());

        string result = combat.GetCombatStatsDisplay();

        CollectionAssert.Contains(result.Split(Environment.NewLine), "Armor: Magical Armor (absorbs 3 damage)");
    }

    [TestMethod]
    public void GetCombatStatsDisplay_AfterBuyingMagicalWeapon_ShowsMagicalNameAndNewMaxDamage()
    {
        Player player = CreateTestPlayer();
        player.Occupation = "Magician";
        player.Gold = 20;
        player.Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
        new Merchant().Purchase(player, MerchantItem.MagicalWeapon);
        Combat combat = CreateCombat(player, CreateTestDragon(), CreateEnglishMessages());

        string[] lines = combat.GetCombatStatsDisplay().Split(Environment.NewLine);

        CollectionAssert.Contains(lines, "Weapon: magical lightning bolt spell");
        CollectionAssert.Contains(lines, "Weapon's Maximum Damage Points: 16");
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ForNewPlayer_ShowsLeatherArmorLine()
    {
        Combat combat = CreateCombat(CreateTestPlayer(), CreateTestDragon(), CreateEnglishMessages());

        string result = combat.GetCombatStatsDisplay();

        CollectionAssert.Contains(result.Split(Environment.NewLine), "Armor: Leather Armor (absorbs 0 damage)");
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = CreateCombat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }
}
