using Godot;

public partial class Player : CharacterBody2D
{
    // ==========================================
    // Movement
    // ==========================================

    private float _speed = 300f;

    // ==========================================
    // Bullet
    // ==========================================

    private PackedScene _bulletScene = GD.Load<PackedScene>("res://bullet.tscn");

    // ==========================================
    // Aim Mode
    // ==========================================

    private bool _isAutoAim = false;

    // ==========================================
    // Health
    // ==========================================

    private int _maxHealth = 3;
    private int _currentHealth;
    private bool _isInvincible = false;
    private bool _isDead = false;

    // ==========================================
    // Weapon
    // ==========================================

    private WeaponManager _weaponManager;
    private bool _canShoot = true;

    // ==========================================
    // Reload System
    // ==========================================

    private bool _isReloading = false;

    // Used to invalidate old reload timers
    private int _reloadVersion = 0;

    // ==========================================
    // Ready
    // ==========================================

    public override void _Ready()
    {
        GD.Print("Player _Ready() executed!");
        AddToGroup("player");

        _weaponManager = GetNode<WeaponManager>("WeaponManager");

        if (_weaponManager.CurrentWeapon != null)
        {
            GD.Print($"Current Weapon: {_weaponManager.CurrentWeapon.Name}");
        }

        if (_weaponManager != null)
        {
            GD.Print("WeaponManager node found!");
        }
        else
        {
            GD.Print("WeaponManager node NOT found!");
        }

        _currentHealth = _maxHealth;

        UpdateHealthUI();
        QueueRedraw();

        GD.Print("Manual Aim Enabled");
    }

    // ==========================================
    // Movement System
    // ==========================================

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
            return;

        Vector2 velocity = Vector2.Zero;

        if (Input.IsKeyPressed(Key.W))
            velocity.Y -= 1;

        if (Input.IsKeyPressed(Key.S))
            velocity.Y += 1;

        if (Input.IsKeyPressed(Key.A))
            velocity.X -= 1;

        if (Input.IsKeyPressed(Key.D))
            velocity.X += 1;

        if (velocity != Vector2.Zero)
            velocity = velocity.Normalized();

        Velocity = velocity * _speed;

        MoveAndSlide();
    }

    // ==========================================
    // Health System
    // ==========================================

    public void TakeDamage()
    {
        if (_isInvincible || _isDead)
            return;

        _currentHealth -= 1;

        GD.Print($"玩家受伤！剩余生命: {_currentHealth}");

        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            _isDead = true;
            NotifyGameManager();
        }
        else
        {
            // 1-second invincibility
            _isInvincible = true;

            GetTree().CreateTimer(1.0).Timeout += () =>
            {
                _isInvincible = false;
            };

            // Damage visual effect
            Modulate = new Color(1, 0.5f, 0.5f);

            GetTree().CreateTimer(0.2).Timeout += () =>
            {
                Modulate = Colors.White;
            };
        }
    }

    // Notify GameManager when player dies
    private void NotifyGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            var gameManager = GetNodeOrNull<GameManager>("/root/GameManager") 
                ?? GetTree().CurrentScene?.GetNodeOrNull<GameManager>("GameManager");

            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            else
            {
                GD.PushError("GameManager was not found!");
            }
        }
    }

    // Update Health UI
    private void UpdateHealthUI()
    {
        var hpLabel = GetTree().Root.FindChild("HealthLabel", true, false) as Label;

        if (hpLabel != null)
            hpLabel.Text = $"HP: {_currentHealth}";
    }

    // ==========================================
    // Input System
    // ==========================================

    public override void _Input(InputEvent @event)
    {
        if (_isDead)
            return;

        // Toggle Aim Mode using Tab
        if (
            @event is InputEventKey keyEvent
            && keyEvent.Pressed
            && !keyEvent.Echo
            && keyEvent.Keycode == Key.Tab
        )
        {
            _isAutoAim = !_isAutoAim;

            GD.Print(_isAutoAim ? "Auto Aim Enabled" : "Manual Aim Enabled");
        }

        // Press R to reload
        if (
            @event is InputEventKey reloadEvent
            && reloadEvent.Pressed
            && !reloadEvent.Echo
            && reloadEvent.Keycode == Key.R
        )
        {
            StartReload();
        }

        // Left Mouse Button to shoot
        if (
            @event is InputEventMouseButton mouseEvent
            && mouseEvent.Pressed
            && mouseEvent.ButtonIndex == MouseButton.Left
        )
        {
            Shoot();
        }
    }

    // ==========================================
    // Enemy Detection
    // ==========================================

    private Enemy FindNearestEnemy()
    {
        Enemy nearestEnemy = null;

        float nearestDistance = float.MaxValue;

        foreach (Node node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Enemy enemy || !GodotObject.IsInstanceValid(enemy))
            {
                continue;
            }

            float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    // ==========================================
    // Shooting System
    // ==========================================

    private void Shoot()
    {
        // Check current weapon
        Weapon weapon = _weaponManager?.CurrentWeapon;

        if (weapon == null)
            return;

        // Interrupt reload when player tries to shoot
        if (_isReloading)
        {
            _isReloading = false;
            _reloadVersion++;

            GD.Print("Reload interrupted!");
        }

        // Check shooting cooldown
        if (!_canShoot)
            return;

        // Check bullet scene
        if (_bulletScene == null)
            return;

        // Check ammo
        if (!weapon.HasEnoughAmmo())
        {
            GD.Print("Out of ammo!!");
            return;
        }

        Vector2 direction = Vector2.Zero;

        if (_isAutoAim)
        {
            Enemy target = FindNearestEnemy();

            // Stop shooting if no enemy exists
            if (target == null)
                return;

            direction = (target.GlobalPosition - GlobalPosition).Normalized();
        }
        else
        {
            Vector2 mousePos = GetGlobalMousePosition();

            direction = (mousePos - GlobalPosition).Normalized();
        }

        // Prevent invalid shooting direction
        if (direction == Vector2.Zero)
            return;

        // Consume ammo
        if (!weapon.TryConsumeAmmo())
            return;

        GD.Print($"Ammo: {weapon.CurrentAmmo}/{weapon.MagazineSize}");

        // Create bullet
        var bullet = _bulletScene.Instantiate<Bullet>();

        bullet.GlobalPosition = GlobalPosition + direction * 35;

        bullet.Direction = direction;

        GetParent().AddChild(bullet);

        // Start Fire Rate cooldown
        _canShoot = false;

        GetTree().CreateTimer(weapon.FireRate).Timeout += () =>
        {
            _canShoot = true;
        };
    }

    // ==========================================
    // Reload System
    // ==========================================

    private void StartReload()
    {
        Weapon weapon = _weaponManager?.CurrentWeapon;

        if (weapon == null)
            return;

        // Prevent multiple reloads
        if (_isReloading)
            return;

        // Check magazine
        if (weapon.CurrentAmmo >= weapon.MagazineSize)
        {
            GD.Print("Magazine is already full!");
            return;
        }

        // Check reserve ammo
        if (weapon.ReserveAmmo <= 0)
        {
            GD.Print("No reserve ammo!");
            return;
        }

        _isReloading = true;

        _reloadVersion++;

        int currentVersion = _reloadVersion;

        GD.Print($"Reloading {weapon.Name}...");

        // Shotgun uses shell-by-shell reload
        if (weapon.Name == "Shotgun")
        {
            ReloadShotgunShell(currentVersion);
        }
        else
        {
            ReloadNormalWeapon(weapon, currentVersion);
        }
    }

    // ==========================================
    // Normal Weapon Reload
    // ==========================================

    private void ReloadNormalWeapon(Weapon weapon, int currentVersion)
    {
        GetTree().CreateTimer(weapon.ReloadTime).Timeout += () =>
        {
            // Check whether reload was interrupted
            if (!_isReloading || currentVersion != _reloadVersion)
            {
                return;
            }

            weapon.ReloadAmmo();

            _isReloading = false;

            GD.Print(
                $"Reload complete! Ammo: {weapon.CurrentAmmo}/{weapon.MagazineSize}, Reserve: {weapon.ReserveAmmo}"
            );
        };
    }

    // ==========================================
    // Shotgun Reload
    // ==========================================

    private void ReloadShotgunShell(int currentVersion)
    {
        Weapon weapon = _weaponManager?.CurrentWeapon;

        if (weapon == null)
            return;

        // Stop when magazine is full or reserve ammo is empty
        if (weapon.CurrentAmmo >= weapon.MagazineSize || weapon.ReserveAmmo <= 0)
        {
            _isReloading = false;

            GD.Print(
                $"Shotgun reload stopped! Ammo: {weapon.CurrentAmmo}/{weapon.MagazineSize}, Reserve: {weapon.ReserveAmmo}"
            );

            return;
        }

        // Load one shell after ReloadTime
        GetTree().CreateTimer(weapon.ReloadTime).Timeout += () =>
        {
            // Check whether reload was interrupted
            if (!_isReloading || currentVersion != _reloadVersion)
            {
                return;
            }

            // Load one shell
            if (!weapon.ReloadOneAmmo())
            {
                _isReloading = false;
                return;
            }

            GD.Print(
                $"Shotgun Reload: {weapon.CurrentAmmo}/{weapon.MagazineSize}, Reserve: {weapon.ReserveAmmo}"
            );

            // Continue loading the next shell
            ReloadShotgunShell(currentVersion);
        };
    }

    // ==========================================
    // Draw Player
    // ==========================================

    public override void _Draw()
    {
        DrawRect(new Rect2(-25, -25, 50, 50), Colors.Cyan);
    }
}
