[TestClass]
[DoNotParallelize]
public class InventoryTests
{
    // Inputs that walk a game in the given language (1-4) up to the path choice, as a Fighter.
    private static string ToPath(string language = "1", string race = "Human", string occupation = "Fighter") =>
        $"{language}\n1\n{race}\nHero\n{occupation}\nroll\nroll\nroll\n2\n";

    private const string PathPrompt = "Which do you take? Enter 'n' for North.";
    private const string MerchantPrompt = "Enter 'a' for armor, 'w' for weapon, or 'l' to leave.";
    private const string ShrinePrompt = "Enter 'o' to make an offering, 'p' to pray, or 'l' to leave.";
    private const string DragonPrompt = "Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.";

    // How many times the text appears in the output.
    private static int CountOf(string output, string text)
    {
        int count = 0;
        for (int i = output.IndexOf(text, StringComparison.Ordinal); i >= 0; i = output.IndexOf(text, i + 1, StringComparison.Ordinal))
            count++;
        return count;
    }

    // Runs a whole game on scripted console input and returns everything written to the console.
    private static string PlayGame(string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        var output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader(input));
            Console.SetOut(output);
            new Game().StartGame();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        return output.ToString();
    }

    // Asserts that each text appears in the output, each after the one before it.
    private static void AssertInOrder(string output, params string[] texts)
    {
        int previous = -1;
        foreach (string text in texts)
        {
            int index = output.IndexOf(text, previous + 1, StringComparison.Ordinal);
            Assert.IsTrue(index > previous, $"Expected \"{text}\" after the previous step.\nOutput:\n{output}");
            previous = index;
        }
    }

    [TestMethod]
    [Timeout(5000)]
    public void CheckingInventoryAtPathPrompt_NewFighter_ShowsInventoryThenPathPromptAgain()
    {
        string output = PlayGame(ToPath() + "i\ne\n");

        AssertInOrder(output,
            PathPrompt,
            "----- Inventory -----",
            "Weapon: long sword",
            "Weapon's Maximum Damage Points: 12",
            "Armor: Leather Armor (absorbs 0 damage)",
            "Gold: 20",
            PathPrompt,
            "Adventure ends. Bye.");
        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex("Invalid"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void CheckingInventoryAfterBuyingMagicalArmor_ShowsMagicalArmorAndRemainingGold()
    {
        string output = PlayGame(ToPath() + "s\na\ni\ne\n");

        AssertInOrder(output,
            "----- Inventory -----",
            "Weapon: long sword",
            "Weapon's Maximum Damage Points: 12",
            "Armor: Magical Armor (absorbs 3 damage)",
            "Gold: 5",
            PathPrompt);
    }

    [TestMethod]
    [Timeout(5000)]
    public void CheckingInventoryAfterBuyingMagicalWeapon_ShowsMagicalWeaponItsDamageAndZeroGold()
    {
        string output = PlayGame(ToPath() + "s\nw\ni\ne\n");

        AssertInOrder(output,
            "----- Inventory -----",
            "Weapon: magical long sword",
            "Weapon's Maximum Damage Points: 16",
            "Armor: Leather Armor (absorbs 0 damage)",
            "Gold: 0",
            PathPrompt);
    }

    [TestMethod]
    [Timeout(5000)]
    public void FullInventoryWord_WithMixedCaseAndSurroundingSpaces_ShowsInventory()
    {
        string output = PlayGame(ToPath() + "  InVenTory  \ne\n");

        AssertInOrder(output, PathPrompt, "----- Inventory -----", "Gold: 20", PathPrompt);
        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex("Invalid"));
    }

    [TestMethod]
    [Timeout(5000)]
    [DataRow("2", "Humano", "Guerrero", "inventario", "----- Inventario -----", "Oro: 20", DisplayName = "Spanish")]
    [DataRow("3", "Humain", "Guerrier", "inventaire", "----- Inventaire -----", "Or: 20", DisplayName = "French")]
    [DataRow("4", "Umano", "Guerriero", "inventario", "----- Inventario -----", "Oro: 20", DisplayName = "Italian")]
    public void FullInventoryWord_InTranslatedLanguage_ShowsTranslatedHeading(
        string language, string race, string occupation, string word, string heading, string goldLine)
    {
        string output = PlayGame(ToPath(language, race, occupation) + word + "\ne\n");

        AssertInOrder(output, heading, goldLine);
    }

    [TestMethod]
    [Timeout(5000)]
    public void CheckingInventoryAtMerchantPrompt_ShowsInventoryThenMerchantPromptAgain_AndLeavingStillWorks()
    {
        string output = PlayGame(ToPath() + "s\ni\nl\ne\n");

        AssertInOrder(output,
            MerchantPrompt,
            "----- Inventory -----",
            "Weapon: long sword",
            "Armor: Leather Armor (absorbs 0 damage)",
            "Gold: 20",
            MerchantPrompt,
            "You thank Oswin and walk away.",
            PathPrompt);
        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex("Invalid"));
        Assert.AreEqual(1, CountOf(output, "a small wooden stall"), "The Merchant intro should not be shown again.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void CheckingInventoryAtShrinePrompt_ShowsInventoryThenShrinePromptAgain_AndWalkingPastStillWorks()
    {
        string output = PlayGame(ToPath() + "n\ni\nl\nr\n");

        AssertInOrder(output,
            ShrinePrompt,
            "----- Inventory -----",
            "Weapon: long sword",
            "Armor: Leather Armor (absorbs 0 damage)",
            "Gold: 20",
            ShrinePrompt,
            "You bow your head to the Shrine and walk past.",
            DragonPrompt);
        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex("Invalid"));
        Assert.AreEqual(1, CountOf(output, "an old stone Shrine"), "The Shrine intro should not be shown again.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void FullInventoryWordInFrench_AtShrinePrompt_ShowsInventoryThenShrinePromptAgain()
    {
        const string frenchShrinePrompt = "Tapez 'o' pour faire une offrande, 'p' pour prier ou 'l' pour partir.";

        string output = PlayGame(ToPath("3", "Humain", "Guerrier") + "n\ninventaire\npartir\nr\n");

        AssertInOrder(output,
            frenchShrinePrompt,
            "----- Inventaire -----",
            "Or: 20",
            frenchShrinePrompt,
            "Vous inclinez la tete devant le Sanctuaire et passez votre chemin.");
        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex("Choix invalide"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void CheckingInventoryAtDragonPrompt_ShowsInventoryThenDragonPromptAgain_AndRetreatingStillWorks()
    {
        string output = PlayGame(ToPath() + "n\nl\ni\nr\n");

        AssertInOrder(output,
            DragonPrompt,
            "----- Inventory -----",
            "Weapon: long sword",
            "Armor: Leather Armor (absorbs 0 damage)",
            "Gold: 20",
            DragonPrompt,
            "You quickly retreat.");
        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex("Invalid"));
        Assert.AreEqual(1, CountOf(output, "You are confronted with a dragon"), "The dragon arrival should not be shown again.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void PathPrompt_ShowsInventoryHintBeforeEnterChoice()
    {
        string output = PlayGame(ToPath() + "e\n");

        string nl = Environment.NewLine;
        AssertInOrder(output, PathPrompt + $" Enter 's' for South. Enter 'e' for exit game.{nl}Enter 'i' to check your inventory.{nl}Enter choice: ");
    }

    [TestMethod]
    [Timeout(5000)]
    [DataRow("s\nl\ne\n", MerchantPrompt, DisplayName = "Merchant")]
    [DataRow("n\nl\nr\n", ShrinePrompt, DisplayName = "Shrine")]
    [DataRow("n\nl\nr\n", DragonPrompt, DisplayName = "dragon encounter")]
    public void MerchantShrineAndDragonPrompts_ShowInventoryHintBeforeEnterChoice(string inputAfterPath, string prompt)
    {
        string output = PlayGame(ToPath() + inputAfterPath);

        string nl = Environment.NewLine;
        AssertInOrder(output, $"{prompt}{nl}Enter 'i' to check your inventory.{nl}Enter choice: ");
    }
}
