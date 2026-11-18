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
    PrayerAnswered,
    PrayerDispleased,
    WalkedPast
}

// The Shrine: the Detour met on the North path before the dragon.
// One Shrine is created per North trip, so it keeps no state between visits.
public class Shrine
{
    public const int OfferingCost = 5;
    public const int OfferingHealing = 5;
    public const int PrayerHealthChange = 2;
    public const int PrayerDieSides = 6;
    public const int PrayerDispleasedMaxRoll = 2;
    public const int PrayerMinimumHealthPoints = 1;

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
        ShrineChoice.Pray => Pray(player),
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

    // Prayer is a gamble on a d6 and only changes Health Points, never Gold.
    // Healing has no upper cap; harm never takes the player below 1 Health Point,
    // so the Shrine itself can never kill the player.
    private ShrineResult Pray(Player player)
    {
        if (_die.Roll(PrayerDieSides) <= PrayerDispleasedMaxRoll)
        {
            player.HealthPoints = Math.Max(PrayerMinimumHealthPoints, player.HealthPoints - PrayerHealthChange);
            return ShrineResult.PrayerDispleased;
        }

        player.HealthPoints += PrayerHealthChange;
        return ShrineResult.PrayerAnswered;
    }

    public void Visit(Player player, Messages messages, Prompt prompt)
    {
        Console.WriteLine(messages.GetMessage("shrine_intro"));

        // "Not enough Gold" asks again so the player can choose something else.
        while (true)
        {
            ShrineChoice choice = Enum.Parse<ShrineChoice>(prompt.Ask(
                questionKey: "shrine_prompt",
                choiceMap: "shrineChoiceMap",
                invalidKey: "invalid",
                offerInventoryOf: player), ignoreCase: true);

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

            if (choice == ShrineChoice.Pray)
            {
                string outcomeKey = Choose(player, ShrineChoice.Pray) == ShrineResult.PrayerAnswered
                    ? "shrine_prayer_answered"
                    : "shrine_prayer_displeased";
                Console.WriteLine(messages.GetMessage(outcomeKey));
                Console.WriteLine(string.Format(messages.GetMessage("shrine_health_now"), player.HealthPoints));
                return;
            }

            Choose(player, ShrineChoice.Leave);
            Console.WriteLine(messages.GetMessage("shrine_leave"));
            return;
        }
    }
}
