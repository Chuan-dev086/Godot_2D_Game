public class Weapon
{
    public string Name { get; set; }

    // Combat Properties
    public int Damage { get; set; }
    public float FireRate { get; set; }
    public float BulletSpeed { get; set; }
    public int BulletCount { get; set; }

    // Ammo Properties
    public int MagazineSize { get; set; }
    public int CurrentAmmo { get; set; }
    public int ReserveAmmo { get; set; }
    public int AmmoPerShot { get; set; }

    // Reload Properties
    public float ReloadTime { get; set; }

    // Special Properties
    public bool CanPierce { get; set; }

    public Weapon(
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
        Name = name;

        Damage = damage;
        FireRate = fireRate;
        BulletSpeed = bulletSpeed;
        BulletCount = bulletCount;

        MagazineSize = magazineSize;
        CurrentAmmo = magazineSize;
        ReserveAmmo = reserveAmmo;
        AmmoPerShot = ammoPerShot;

        ReloadTime = reloadTime;

        CanPierce = canPierce;
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
