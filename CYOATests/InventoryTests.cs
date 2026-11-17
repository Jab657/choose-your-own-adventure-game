[TestClass]
[DoNotParallelize]
public class InventoryTests
{
    // Inputs that walk a game in the given language (1-4) up to the path choice, as a Fighter.
    private static string ToPath(string language = "1", string race = "Human", string occupation = "Fighter") =>
        $"{language}\n1\n{race}\nHero\n{occupation}\nroll\nroll\nroll\n2\n";

    private const string PathPrompt = "Which do you take? Enter 'n' for North.";

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
    public void PathPrompt_ShowsInventoryHintBeforeEnterChoice()
    {
        string output = PlayGame(ToPath() + "e\n");

        string nl = Environment.NewLine;
        AssertInOrder(output, PathPrompt + $" Enter 's' for South. Enter 'e' for exit game.{nl}Enter 'i' to check your inventory.{nl}Enter choice: ");
    }
}
