public enum MerchantItem
{
    MagicalArmor,
    MagicalWeapon
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

        if (item == MerchantItem.MagicalWeapon && player.Weapon == null)
            throw new InvalidOperationException("The Magical Weapon enchants the player's own weapon, but the player has none.");

        player.Gold -= price;
        switch (item)
        {
            case MerchantItem.MagicalArmor:
                player.Armor = Armor.MagicalArmor();
                break;
            case MerchantItem.MagicalWeapon:
                player.Weapon!.Enchant();
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
        MerchantItem.MagicalWeapon => MagicalWeaponPrice,
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

            MerchantItem? item = choice switch
            {
                "armor" => MerchantItem.MagicalArmor,
                "weapon" => MerchantItem.MagicalWeapon,
                _ => null
            };

            if (item != null)
            {
                // A visit only reaches the menu before trading, so AlreadyTraded cannot happen here.
                PurchaseResult result = Purchase(player, item.Value);
                if (result == PurchaseResult.Bought)
                {
                    Console.WriteLine(BoughtMessage(item.Value, player, messages));
                    return;
                }

                if (result == PurchaseResult.NotEnoughGold)
                {
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_not_enough_gold"), PriceOf(item.Value), player.Gold));
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

    private static string BoughtMessage(MerchantItem item, Player player, Messages messages) => item switch
    {
        MerchantItem.MagicalArmor => string.Format(messages.GetMessage("merchant_bought_armor"), MagicalArmorPrice),
        MerchantItem.MagicalWeapon => string.Format(messages.GetMessage("merchant_bought_weapon"), MagicalWeaponPrice,
            messages.TranslateWeaponForDisplay(player.Weapon!.Type)),
        _ => throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown Merchant item.")
    };
}
