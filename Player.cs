using Godot;

public partial class Player : CharacterBody2D
{
    private float _speed = 300f;
    private PackedScene _bulletScene = GD.Load<PackedScene>("res://bullet.tscn");

    // Aim Mode
    private bool _isAutoAim = false;

    // Health
    private int _maxHealth = 3;
    private int _currentHealth;
    private bool _isInvincible = false;

    public override void _Ready()
    {
        _currentHealth = _maxHealth;
        UpdateHealthUI();
        QueueRedraw();

        GD.Print("Manual Aim Enabled");
    }

    public override void _PhysicsProcess(double delta)
    {
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

        var cam = GetParent().GetNodeOrNull<Camera2D>("Camera2D");

        if (cam != null)
            cam.GlobalPosition = GlobalPosition;
    }

    // Called when the player takes damage
    public void TakeDamage()
    {
        if (_isInvincible)
            return;

        _currentHealth -= 1;

        GD.Print($"玩家受伤！剩余生命: {_currentHealth}");

        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
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

    // Notify GameManager when the player dies
    private void NotifyGameManager()
    {
        var gameManager = GetTree().CurrentScene.GetNodeOrNull<GameManager>("GameManager");

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            GD.PushError("GameManager node was not found!");
        }
    }

    private void UpdateHealthUI()
    {
        var hpLabel = GetTree().Root.FindChild("HealthLabel", true, false) as Label;

        if (hpLabel != null)
            hpLabel.Text = $"HP: {_currentHealth}";
    }

    // Handle input
    public override void _Input(InputEvent @event)
    {
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

        // Left Mouse Button to shoot in both modes
        if (
            @event is InputEventMouseButton mouseEvent
            && mouseEvent.Pressed
            && mouseEvent.ButtonIndex == MouseButton.Left
        )
        {
            Shoot();
        }
    }

    // Find the nearest enemy
    private Enemy FindNearestEnemy()
    {
        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Node node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Enemy enemy || !GodotObject.IsInstanceValid(enemy))
                continue;

            float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    // Handle shooting
    private void Shoot()
    {
        if (_bulletScene == null)
            return;

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

        // Prevent shooting with an invalid direction
        if (direction == Vector2.Zero)
            return;

        var bullet = _bulletScene.Instantiate<Bullet>();

        bullet.GlobalPosition = GlobalPosition + direction * 35;
        bullet.Direction = direction;

        GetParent().AddChild(bullet);
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(-25, -25, 50, 50), Colors.Cyan);
    }
}
