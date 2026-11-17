// CYOATests/NorthPathTests.cs
[TestClass]
[DoNotParallelize]
public class NorthPathTests
{
    // Inputs that walk an English game up to the path choice.
    private const string ToPath = "1\n1\nHuman\nHero\nFighter\nroll\nroll\nroll\n2\n";

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

    [TestMethod]
    [Timeout(5000)]
    public void GoingNorthAndWalkingPastTheShrine_ShowsJourneyThenShrineThenDragon()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        // Walk past the Shrine, then retreat from the dragon so no dice decide the outcome.
        string output = PlayGame(ToPath + "n\nl\nr\n");

        string[] inOrder =
        {
            messages.GetMessage("north_path_narrative"),
            messages.GetMessage("shrine_intro"),
            messages.GetMessage("shrine_leave"),
            messages.GetMessage("dragon_arrival"),
            messages.GetMessage("dragon_stats_intro"),
            messages.GetMessage("dragon_encounter_prompt"),
            messages.GetMessage("retreat")
        };

        int previous = -1;
        foreach (string text in inOrder)
        {
            int index = output.IndexOf(text, previous + 1, StringComparison.Ordinal);
            Assert.IsTrue(index > previous, $"Expected \"{text}\" after the previous step.");
            previous = index;
        }
    }
}
