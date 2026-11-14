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
    [DataRow("English", "Weapon: magical dagger", "Weapon's Maximum Damage Points: 10")]
    [DataRow("Spanish", "Arma: daga magica", "Puntos Maximos de Dano del Arma: 10")]
    [DataRow("French", "Arme: dague magique", "Points de Dommages Maximum: 10")]
    [DataRow("Italian", "Arma: pugnale magico", "Danno Massimo dell'Arma: 10")]
    public void DisplayStats_AfterBuyingMagicalWeapon_ShowsMagicalNameAndNewMaxDamage(string language, string weaponLine, string damageLine)
    {
        Messages messages = CreateMessages(language);
        Player player = new Player { Name = "Aria", Race = "Halfling", Occupation = "Thief", Gold = 20,
                                     Weapon = new Weapon("dagger", 6, "-)==>") };
        new Merchant().Purchase(player, MerchantItem.MagicalWeapon);

        string output = RunWithConsole("", () => player.DisplayStats(messages));

        string[] lines = output.Split(Environment.NewLine);
        CollectionAssert.Contains(lines, weaponLine);
        CollectionAssert.Contains(lines, damageLine);
    }

    [TestMethod]
    [DataRow("English", "As a Archer, you withdraw your magical long bow and attack the dragon.")]
    [DataRow("Spanish", "Como Arquero, sacas tu arco largo magico y atacas al dragon.")]
    [DataRow("French", "En tant que Archer, vous sortez votre arc long magique et attaquez le dragon.")]
    [DataRow("Italian", "Come Arciere, estrai il tuo arco lungo magico e attacchi il drago.")]
    public void Attack_AfterBuyingMagicalWeapon_NamesTheMagicalWeapon(string language, string expectedIntro)
    {
        Messages messages = CreateMessages(language);
        Player player = new Player { Name = "Aria", Race = "Elf", Occupation = "Archer", Strength = 10, Gold = 20,
                                     Weapon = new Weapon("long bow", 8, "}    -->") };
        new Merchant().Purchase(player, MerchantItem.MagicalWeapon);
        Dragon dragon = new Dragon(new Die()) { Name = "Smolderfang", Agility = 10, HealthPoints = 30,
                                                Weapon = new Weapon("claws", 12, "<<< claws >>>") };

        string output = player.Attack(dragon, messages);

        Assert.AreEqual(expectedIntro, output.Split(Environment.NewLine)[0]);
        Assert.AreEqual("}    -->", output.Split(Environment.NewLine)[1]);
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
