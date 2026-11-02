using System.Collections.Generic;
using Godot;

public partial class WeaponManager : Node
{
    // Store available weapons
    private List<Weapon> _weapons = new List<Weapon>();

    // Current equipped weapon
    public Weapon CurrentWeapon { get; private set; }

    // Reference to GameSettings Autoload
    private GameSettings _gameSettings;

    public override void _Ready()
    {
        GD.Print("WeaponManager _Ready() executed!");

        // Get GameSettings Autoload
        _gameSettings = GetNode<GameSettings>("/root/GameSettings");

        // Register available weapons
        _weapons.Add(new Weapon("Pistol", 1, 0.4f, 800f, 1, 12, 60, 1, 1.2f, false));

        _weapons.Add(new Weapon("SMG", 1, 0.1f, 900f, 1, 30, 120, 1, 1.8f, false));

        _weapons.Add(new Weapon("Shotgun", 2, 0.8f, 600f, 5, 15, 75, 5, 1.0f, false));

        _weapons.Add(new Weapon("Sniper", 10, 1.5f, 1500f, 1, 5, 15, 1, 4.0f, true));

        // Get the weapon selected by the player
        string selectedWeapon = _gameSettings.InitialWeapon;

        // Equip the selected weapon
        EquipWeapon(selectedWeapon);

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
