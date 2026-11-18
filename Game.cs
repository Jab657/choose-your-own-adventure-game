public class Game
{
    public void StartGame()
    {
        try
        {
            PlayGame();
        }
        catch (EndOfInputException)
        {
            // Input ended at a prompt: stop the game instead of re-prompting forever.
            // The newline keeps the shell prompt off the unanswered game prompt.
            Console.WriteLine();
        }
    }

    private void PlayGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        Dragon dragon = new Dragon(die);

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Scegli la lingua:");
        Console.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        Console.Write("Enter choice: ");
        string langInput = ConsoleInput.ReadLine().Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();
        Prompt prompt = new Prompt(messages);
        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine(messages.GetMessage("menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string choice = ConsoleInput.ReadLine().Trim();

            if (choice == "1")
            {
                Player player = new Player();
                player.CreateCharacter(messages);
                Merchant merchant = new Merchant();

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = prompt.Ask(
                        questionKey: "path_prompt_full",
                        choiceMap: "pathChoiceMap",
                        invalidKey: "path_invalid",
                        offerInventoryOf: player);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        merchant.Visit(player, messages, prompt);
                    }
                    else if (selectedPath == "north")
                    {
                        Console.WriteLine(messages.GetMessage("north_path_narrative"));
                        new Shrine(die).Visit(player, messages, prompt);
                        HandleDragonEncounter(player, dragon, messages, prompt);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            Console.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            Console.WriteLine(messages.GetMessage("victory"));
        else
            Console.WriteLine(messages.GetMessage("defeat"));
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "e" || trimmed == "exit";
    }

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "r" || trimmed == "retreat";
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages, Prompt prompt)
    {
        Console.WriteLine(messages.GetMessage("dragon_arrival"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);

        string choice = prompt.Ask(
            questionKey: "dragon_encounter_prompt",
            choiceMap: "dragonChoiceMap",
            invalidKey: "invalid",
            offerInventoryOf: player);

        if (choice == "retreat")
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        Combat combat = new Combat(player, dragon, messages);
        bool playerWon = combat.StartCombat();

        // Combat has already shown the retreat message.
        if (!combat.PlayerRetreated)
            EndGame(playerWon, false, messages);
    }
}
