// A Prompt: a question the game asks the player, repeated until they give one of its accepted answers.
public class Prompt
{
    private readonly Messages _messages;

    public Prompt(Messages messages)
    {
        _messages = messages;
    }

    // Asks the question until the answer is in the choice map, and returns that answer's canonical English value.
    // With a player, the Prompt also offers that player's Inventory: checking it is free and asks the question again.
    // With sameLine, the answer is typed after "Enter choice:" on the same line; offering the Inventory implies it.
    // Throws EndOfInputException when input ends.
    public string Ask(string questionKey, string choiceMap, string invalidKey,
        Player? offerInventoryOf = null, bool sameLine = false)
    {
        while (true)
        {
            Console.WriteLine(_messages.GetMessage(questionKey));
            if (offerInventoryOf != null)
                Console.WriteLine(_messages.GetMessage("inventory_hint"));
            if (offerInventoryOf != null || sameLine)
                Console.Write(_messages.GetMessage("enter_choice"));
            string answer = ConsoleInput.ReadLine().Trim().ToLower();

            if (offerInventoryOf != null && _messages.LookupChoice("inventoryCommandMap", answer) != null)
            {
                offerInventoryOf.DisplayInventory(_messages);
                continue;
            }

            string? canonical = _messages.LookupChoice(choiceMap, answer);
            if (canonical != null)
                return canonical;

            Console.WriteLine(_messages.GetMessage(invalidKey));
        }
    }
}
