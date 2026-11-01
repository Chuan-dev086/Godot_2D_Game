public class Weapon
{
    public string Name { get; set; }

    public int Damage { get; set; }

    public float FireRate { get; set; }

    public float BulletSpeed { get; set; }

    public int BulletCount { get; set; }

    public Weapon(string name, int damage, float fireRate, float bulletSpeed, int bulletCount)
    {
        Name = name;
        Damage = damage;
        FireRate = fireRate;
        BulletSpeed = bulletSpeed;
        BulletCount = bulletCount;
    }
}
