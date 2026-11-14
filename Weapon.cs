public class Weapon
{
    public const int EnchantmentBonus = 4;

    public string Type { get; set; }
    public int MaxDamage { get; set; }
    public string AsciiArt { get; set; }

    public Weapon(string type, int maxDamage, string asciiArt)
    {
        Type = type;
        MaxDamage = maxDamage;
        AsciiArt = asciiArt;
    }

    // Turns this weapon into its magical variant: "long sword" becomes "magical long sword",
    // whose full translated name lives in each language's weapon display map. ASCII art is kept.
    public void Enchant()
    {
        Type = "magical " + Type;
        MaxDamage += EnchantmentBonus;
    }
}