using System.Collections.Generic;
using Godot;

public partial class WeaponManager : Node
{
    // Store available weapons
    private List<Weapon> _weapons = new List<Weapon>();

    // Current equipped weapon
    public Weapon CurrentWeapon { get; private set; }

    public override void _Ready()
    {
        GD.Print("WeaponManager _Ready() executed!");
        // Create default weapons
        _weapons.Add(new Weapon("Pistol", 1, 0.4f, 800f, 1));
        _weapons.Add(new Weapon("SMG", 1, 0.1f, 900f, 1));
        _weapons.Add(new Weapon("Shotgun", 1, 0.8f, 600f, 5));

        // Equip the first weapon
        EquipWeapon("Pistol");

        GD.Print("WeaponManager is ready!");
    }

    // Equip weapon by name
    public void EquipWeapon(string weaponName)
    {
        Weapon weapon = _weapons.Find(item => item.Name == weaponName);

        if (weapon == null)
        {
            GD.PushWarning($"Weapon not found: {weaponName}");
            return;
        }

        CurrentWeapon = weapon;

        GD.Print($"Equipped Weapon: {CurrentWeapon.Name}");
    }
}
