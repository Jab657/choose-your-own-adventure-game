public enum ShrineChoice
{
    Offer,
    Pray,
    Leave
}

public enum ShrineResult
{
    OfferingMade,
    NotEnoughGold,
    WalkedPast
}

// The Shrine: the Detour met on the North path before the dragon.
// One Shrine is created per North trip, so it keeps no state between visits.
public class Shrine
{
    public const int OfferingCost = 5;
    public const int OfferingHealing = 5;

    private readonly Die _die;

    public Shrine(Die die)
    {
        _die = die;
    }

    // Changes only player state; never writes to the console.
    public ShrineResult Choose(Player player, ShrineChoice choice) => choice switch
    {
        ShrineChoice.Offer => MakeOffering(player),
        ShrineChoice.Leave => ShrineResult.WalkedPast,
        // Prayers are not built yet.
        ShrineChoice.Pray => throw new NotSupportedException($"The Shrine does not handle {choice} yet."),
        _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, "Unknown Shrine choice.")
    };

    // Player state changes only when the result is OfferingMade.
    // Healing has no upper cap: the game has no maximum Health Points.
    private static ShrineResult MakeOffering(Player player)
    {
        if (player.Gold < OfferingCost)
            return ShrineResult.NotEnoughGold;

        player.Gold -= OfferingCost;
        player.HealthPoints += OfferingHealing;
        return ShrineResult.OfferingMade;
    }

    public void Visit(Player player, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("shrine_intro"));

        while (true)
        {
            Console.WriteLine(messages.GetMessage("shrine_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            ShrineChoice? choice = messages.NormalizeShrineChoice(ConsoleInput.ReadLine());

            if (choice == ShrineChoice.Offer)
            {
                if (Choose(player, ShrineChoice.Offer) == ShrineResult.NotEnoughGold)
                {
                    Console.WriteLine(string.Format(messages.GetMessage("shrine_not_enough_gold"), OfferingCost, player.Gold));
                    continue;
                }

                Console.WriteLine(string.Format(messages.GetMessage("shrine_offering_made"), OfferingCost));
                Console.WriteLine(string.Format(messages.GetMessage("shrine_health_now"), player.HealthPoints));
                Console.WriteLine(string.Format(messages.GetMessage("shrine_gold_now"), player.Gold));
                return;
            }

            // Prayers are not built yet, so they are rejected as invalid.
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
