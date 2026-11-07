using Godot;

[GlobalClass]
public partial class Weapon : Resource
{
    [Export]
    public string Name { get; set; } = "";

    // Combat Properties
    [Export]
    public int Damage { get; set; } = 1;

    [Export]
    public float FireRate { get; set; } = 0.2f;

    [Export]
    public float BulletSpeed { get; set; } = 800f;

    [Export]
    public int BulletCount { get; set; } = 1;

    // Ammo Properties
    [Export]
    public int MagazineSize { get; set; } = 10;

    [Export]
    public int CurrentAmmo { get; set; } = 10;

    [Export]
    public int ReserveAmmo { get; set; } = 50;

    [Export]
    public int AmmoPerShot { get; set; } = 1;

    // Reload Properties
    [Export]
    public float ReloadTime { get; set; } = 1.0f;

    // Special Properties
    [Export]
    public bool CanPierce { get; set; } = false;

    public Weapon()
    {
    }

    // Check and consume ammo when shooting
    public bool TryConsumeAmmo()
    {
        if (CurrentAmmo < AmmoPerShot)
            return false;

        CurrentAmmo -= AmmoPerShot;
        return true;
    }

    // Check if the magazine has enough ammo
    public bool HasEnoughAmmo()
    {
        return CurrentAmmo >= AmmoPerShot;
    }

    // Reload ammo from reserve
    public int ReloadAmmo()
    {
        // Calculate how much ammo is missing
        int missingAmmo = MagazineSize - CurrentAmmo;

        // Check how much ammo can be transferred
        int ammoToReload = System.Math.Min(missingAmmo, ReserveAmmo);

        // Transfer reserve ammo to magazine
        CurrentAmmo += ammoToReload;
        ReserveAmmo -= ammoToReload;

        return ammoToReload;
    }

    // Reload one shell (for Shotgun)
    public bool ReloadOneAmmo()
    {
        if (CurrentAmmo >= MagazineSize || ReserveAmmo <= 0)
            return false;

        CurrentAmmo++;
        ReserveAmmo--;

        return true;
    }
}
