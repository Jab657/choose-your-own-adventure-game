public enum ShrineChoice
{
    Offer,
    Pray,
    Leave
}

public enum ShrineResult
{
    WalkedPast
}

// The Shrine: the Detour met on the North path before the dragon.
// One Shrine is created per North trip, so it keeps no state between visits.
public class Shrine
{
    private readonly Die _die;

    public Shrine(Die die)
    {
        _die = die;
    }

    // Changes only player state; never writes to the console.
    public ShrineResult Choose(Player player, ShrineChoice choice) => choice switch
    {
        ShrineChoice.Leave => ShrineResult.WalkedPast,
        // Offerings and prayers are not built yet.
        ShrineChoice.Offer or ShrineChoice.Pray => throw new NotSupportedException($"The Shrine does not handle {choice} yet."),
        _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, "Unknown Shrine choice.")
    };

    public void Visit(Player player, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("shrine_intro"));

        while (true)
        {
            Console.WriteLine(messages.GetMessage("shrine_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            ShrineChoice? choice = messages.NormalizeShrineChoice(ConsoleInput.ReadLine());

            // Offerings and prayers are not built yet, so only walking past is accepted.
            if (choice == ShrineChoice.Leave)
            {
                Choose(player, ShrineChoice.Leave);
                Console.WriteLine(messages.GetMessage("shrine_leave"));
                return;
            }

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }
}
