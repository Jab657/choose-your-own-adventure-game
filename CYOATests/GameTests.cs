// CYOATests/GameTests.cs
[TestClass]
public class GameTests
{
    private Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerWinsInEnglish()
    {
        Game game = new Game();
        game.EndGame(true, false, GetMessages("English"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerLosesInEnglish()
    {
        Game game = new Game();
        game.EndGame(false, false, GetMessages("English"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerWinsInSpanish()
    {
        Game game = new Game();
        game.EndGame(true, false, GetMessages("Spanish"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerWinsInFrench()
    {
        Game game = new Game();
        game.EndGame(true, false, GetMessages("French"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerLosesInSpanish()
    {
        Game game = new Game();
        game.EndGame(false, false, GetMessages("Spanish"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerLosesInFrench()
    {
        Game game = new Game();
        game.EndGame(false, false, GetMessages("French"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerRetreatsInEnglish()
    {
        Game game = new Game();
        game.EndGame(false, true, GetMessages("English"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerRetreatsInSpanish()
    {
        Game game = new Game();
        game.EndGame(false, true, GetMessages("Spanish"));
    }

    [TestMethod]
    public void EndGame_DoesNotThrow_WhenPlayerRetreatsInFrench()
    {
        Game game = new Game();
        game.EndGame(false, true, GetMessages("French"));
    }
}
