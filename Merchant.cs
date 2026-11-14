// Oswin, the Merchant met on the South path. One instance lives for the whole game;
// once the player buys or leaves, the stall is empty for the rest of the game.
public class Merchant
{
    public const int MagicalArmorPrice = 15;
    public const int MagicalWeaponPrice = 20;

    public bool HasTraded { get; private set; }

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
