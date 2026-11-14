// CYOATests/EndOfInputTests.cs
[TestClass]
[DoNotParallelize]
public class EndOfInputTests
{
    // Inputs that walk an English game up to each prompt.
    private const string ToMainMenu = "1\n";
    private const string ToRace = ToMainMenu + "1\n";
    private const string ToName = ToRace + "Human\n";
    private const string ToOccupation = ToName + "Hero\n";
    private const string ToRoll = ToOccupation + "Fighter\n";
    private const string ToNextAction = ToRoll + "roll\nroll\nroll\n";
    private const string ToPath = ToNextAction + "2\n";

    private static readonly TimeSpan HangTimeout = TimeSpan.FromSeconds(5);

    // Runs a whole game on scripted console input. Returns whether StartGame returned
    // before the timeout (false means it was stuck re-prompting) and the console output.
    private static (bool returned, string output) PlayGame(string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        var output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader(input));
            Console.SetOut(output);
            Task game = Task.Run(() => new Game().StartGame());
            return (game.Wait(HangTimeout), output.ToString());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    [DataRow("", DisplayName = "language selection")]
    [DataRow(ToMainMenu, DisplayName = "main menu")]
    [DataRow(ToRace, DisplayName = "race")]
    [DataRow(ToName, DisplayName = "name")]
    [DataRow(ToOccupation, DisplayName = "occupation")]
    [DataRow(ToRoll, DisplayName = "strength roll")]
    [DataRow(ToRoll + "roll\n", DisplayName = "agility roll")]
    [DataRow(ToRoll + "roll\nroll\n", DisplayName = "health roll")]
    [DataRow(ToNextAction, DisplayName = "next action")]
    [DataRow(ToPath, DisplayName = "path choice")]
    [DataRow(ToPath + "s\n", DisplayName = "Merchant")]
    [DataRow(ToPath + "n\n", DisplayName = "dragon encounter")]
    // The dice are random, so combat may end before its prompt; either way the game must return.
    [DataRow(ToPath + "n\na\n", DisplayName = "combat")]
    public void Game_Returns_WhenInputEndsAtPrompt(string input)
    {
        var (returned, _) = PlayGame(input);

        Assert.IsTrue(returned, "StartGame kept running after input ended.");
    }

    [TestMethod]
    public void Game_Returns_WhenInputEndsAfterInvalidChoices()
    {
        var (returned, _) = PlayGame(ToMainMenu + "9\n1\nOrc\nHuman\n\nHero\nWizard\nFighter\nnope\n");

        Assert.IsTrue(returned, "StartGame kept running after input ended.");
    }

    [TestMethod]
    public void InvalidMenuChoice_IsStillRepromptedBeforeExiting()
    {
        var (returned, output) = PlayGame(ToMainMenu + "9\n2\n");

        Assert.IsTrue(returned);
        Messages english = new Messages();
        english.SetCurrentLanguage("English");
        english.ReadDictionary();
        StringAssert.Contains(output, english.GetMessage("invalid"));
        StringAssert.Contains(output, english.GetMessage("goodbye"));
    }
}
