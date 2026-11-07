using System.Collections.Generic;
using Godot;

public partial class WeaponManager : Node
{
    [Signal]
    public delegate void AmmoUpdatedEventHandler(int currentAmmo, int reserveAmmo);

    // Available weapons exported for Godot inspector configuration
    [Export]
    public Weapon[] AvailableWeapons { get; set; }

    // Internal list for fast lookup
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

        // If AvailableWeapons configured in Inspector, use them; otherwise, initialize defaults programmatically
        if (AvailableWeapons != null && AvailableWeapons.Length > 0)
        {
            _weapons.AddRange(AvailableWeapons);
        }
        else
        {
            InitializeDefaultWeapons();
        }

        // Get the weapon selected by the player
        string selectedWeapon = _gameSettings?.InitialWeapon ?? "Pistol";

        // Equip the selected weapon
        EquipWeapon(selectedWeapon);

        GD.Print("WeaponManager is ready!");
    }

    private void InitializeDefaultWeapons()
    {
        _weapons.Add(CreateWeapon("Pistol", 1, 0.4f, 800f, 1, 12, 60, 1, 1.2f, false));
        _weapons.Add(CreateWeapon("SMG", 1, 0.1f, 900f, 1, 30, 120, 1, 1.8f, false));
        _weapons.Add(CreateWeapon("Shotgun", 2, 0.8f, 600f, 5, 15, 75, 5, 1.0f, false));
        _weapons.Add(CreateWeapon("Sniper", 10, 1.5f, 1500f, 1, 5, 15, 1, 4.0f, true));
    }

    private static Weapon CreateWeapon(
        string name,
        int damage,
        float fireRate,
        float bulletSpeed,
        int bulletCount,
        int magazineSize,
        int reserveAmmo,
        int ammoPerShot,
        float reloadTime,
        bool canPierce
    )
    {
        var weapon = new Weapon
        {
            Name = name,
            Damage = damage,
            FireRate = fireRate,
            BulletSpeed = bulletSpeed,
            BulletCount = bulletCount,
            MagazineSize = magazineSize,
            CurrentAmmo = magazineSize,
            ReserveAmmo = reserveAmmo,
            AmmoPerShot = ammoPerShot,
            ReloadTime = reloadTime,
            CanPierce = canPierce
        };
        return weapon;
    }

    // Equip weapon by name
    public void EquipWeapon(string weaponName)
    {
        Weapon weapon = _weapons.Find(item => item != null && item.Name == weaponName);

        if (weapon == null)
        {
            GD.PushWarning($"Weapon not found: {weaponName}");
            if (_weapons.Count > 0 && _weapons[0] != null)
            {
                weapon = _weapons[0];
            }
            else
            {
                return;
            }
        }

        CurrentWeapon = weapon;

        GD.Print($"Equipped Weapon: {CurrentWeapon.Name}");
        NotifyAmmoChanged();
    }

    public void NotifyAmmoChanged()
    {
        if (CurrentWeapon != null)
        {
            EmitSignal(SignalName.AmmoUpdated, CurrentWeapon.CurrentAmmo, CurrentWeapon.ReserveAmmo);
        }
    }
}
