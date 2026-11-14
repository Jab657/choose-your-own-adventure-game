// Magical Armor: absorbs a fixed amount of damage from every hit that gets past the player's defense.
public class Armor
{
    public const int MagicalArmorAbsorption = 3;

    public int Absorption { get; }

    private Armor(int absorption)
    {
        Absorption = absorption;
    }

    public static Armor MagicalArmor() => new Armor(MagicalArmorAbsorption);

    // Damage left after the armor absorbs its share; never below 0, so armor never heals.
    public int Absorb(int damage) => Math.Max(0, damage - Absorption);
}
