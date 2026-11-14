// Every prompt reads through here. When console input ends (Ctrl+Z, or a piped
// script running out of lines) Console.ReadLine returns null; rather than let each
// prompt treat that as an invalid choice and re-prompt forever, we throw
// EndOfInputException, which Game.StartGame catches to end the game cleanly.
public static class ConsoleInput
{
    public static string ReadLine()
    {
        return Console.ReadLine() ?? throw new EndOfInputException();
    }
}

public class EndOfInputException : Exception
{
    public EndOfInputException() : base("Console input ended.") { }
}
