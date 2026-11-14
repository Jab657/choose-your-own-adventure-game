public enum MerchantItem
{
    MagicalArmor
}

public enum PurchaseResult
{
    Bought,
    NotEnoughGold,
    AlreadyTraded
}

// Oswin, the Merchant met on the South path. One instance lives for the whole game;
// once the player buys or leaves, the stall is empty for the rest of the game.
public class Merchant
{
    public const int MagicalArmorPrice = 15;
    public const int MagicalWeaponPrice = 20;

    public bool HasTraded { get; private set; }

    // Player state changes only when the result is Bought.
    public PurchaseResult Purchase(Player player, MerchantItem item)
    {
        if (HasTraded)
            return PurchaseResult.AlreadyTraded;

        int price = PriceOf(item);
        if (player.Gold < price)
            return PurchaseResult.NotEnoughGold;

        player.Gold -= price;
        switch (item)
        {
            case MerchantItem.MagicalArmor:
                player.Armor = Armor.MagicalArmor();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown Merchant item.");
        }

        HasTraded = true;
        return PurchaseResult.Bought;
    }

    private static int PriceOf(MerchantItem item) => item switch
    {
        MerchantItem.MagicalArmor => MagicalArmorPrice,
        _ => throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown Merchant item.")
    };

    public void Visit(Player player, Messages messages)
    {
        if (HasTraded)
        {
            Console.WriteLine(messages.GetMessage("merchant_empty_stall"));
            return;
        }

        Console.WriteLine(messages.GetMessage("merchant_intro"));
        Console.WriteLine(string.Format(messages.GetMessage("merchant_offer_armor"), MagicalArmorPrice));
        Console.WriteLine(string.Format(messages.GetMessage("merchant_offer_weapon"), MagicalWeaponPrice));
        Console.WriteLine(string.Format(messages.GetMessage("merchant_gold"), player.Gold));

        while (true)
        {
            Console.WriteLine(messages.GetMessage("merchant_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? choice = messages.NormalizeMerchantChoice(Console.ReadLine());

            if (choice == "armor")
            {
                // A visit only reaches the menu before trading, so AlreadyTraded cannot happen here.
                PurchaseResult result = Purchase(player, MerchantItem.MagicalArmor);
                if (result == PurchaseResult.Bought)
                {
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_armor"), MagicalArmorPrice));
                    return;
                }

                if (result == PurchaseResult.NotEnoughGold)
                {
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_not_enough_gold"), MagicalArmorPrice, player.Gold));
                    continue;
                }
            }

            if (choice == "leave")
            {
                Console.WriteLine(messages.GetMessage("merchant_leave"));
                HasTraded = true;
                return;
            }

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }
}
