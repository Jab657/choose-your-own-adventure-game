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

    public void Visit(Player player, Messages messages, Prompt prompt)
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

        // "Not enough Gold" asks again so the player can choose something else.
        while (true)
        {
            string choice = prompt.Ask(
                questionKey: "merchant_prompt",
                choiceMap: "merchantChoiceMap",
                invalidKey: "invalid",
                offerInventoryOf: player);

            if (choice == "leave")
            {
                Console.WriteLine(messages.GetMessage("merchant_leave"));
                HasTraded = true;
                return;
            }

            MerchantItem item = choice switch
            {
                "armor" => MerchantItem.MagicalArmor,
                "weapon" => MerchantItem.MagicalWeapon,
                _ => throw new InvalidOperationException($"Unknown Merchant choice '{choice}'.")
            };

            switch (Purchase(player, item))
            {
                case PurchaseResult.Bought:
                    Console.WriteLine(BoughtMessage(item, player, messages));
                    return;
                case PurchaseResult.NotEnoughGold:
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_not_enough_gold"), PriceOf(item), player.Gold));
                    continue;
                default:
                    // A visit only reaches the menu before trading, so AlreadyTraded cannot happen here.
                    throw new InvalidOperationException("The Merchant has already traded.");
            }
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
