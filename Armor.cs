public enum ArmorKind
{
    Leather,
    Magical
}

// The armor the player is wearing. Every character starts in Leather Armor, which buying
// Magical Armor replaces. Armor absorbs a fixed amount of damage from every hit that gets
// past the player's defense.
public class Armor
{
    // Leather Armor is a placeholder that may get a real effect later.
    public const int LeatherArmorAbsorption = 0;
    public const int MagicalArmorAbsorption = 3;

    public ArmorKind Kind { get; }
    public string Name { get; }
    public int Absorption { get; }

    private Armor(ArmorKind kind, string name, int absorption)
    {
        Kind = kind;
        Name = name;
        Absorption = absorption;
    }

    public static Armor LeatherArmor() => new Armor(ArmorKind.Leather, "Leather Armor", LeatherArmorAbsorption);

    public static Armor MagicalArmor() => new Armor(ArmorKind.Magical, "Magical Armor", MagicalArmorAbsorption);

    // Damage left after the armor absorbs its share; never below 0, so armor never heals.
    public int Absorb(int damage) => Math.Max(0, damage - Absorption);
}
