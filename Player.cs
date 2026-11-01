using Godot;

public partial class Player : CharacterBody2D
{
    private float _speed = 300f;
    private PackedScene _bulletScene = GD.Load<PackedScene>("res://bullet.tscn");

    // Health
    private int _maxHealth = 3;
    private int _currentHealth;
    private bool _isInvincible = false;

    public override void _Ready()
    {
        _currentHealth = _maxHealth;
        UpdateHealthUI();
        QueueRedraw();
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

    public override void _Input(InputEvent @event)
    {
        if (
            @event is InputEventMouseButton mouseEvent
            && mouseEvent.Pressed
            && mouseEvent.ButtonIndex == MouseButton.Left
        )
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (_bulletScene == null)
            return;

        var bullet = _bulletScene.Instantiate<Bullet>();

        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 direction = (mousePos - GlobalPosition).Normalized();

        bullet.GlobalPosition = GlobalPosition + direction * 35;
        bullet.Direction = direction;

        GetParent().AddChild(bullet);
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(-25, -25, 50, 50), Colors.Cyan);
    }
}
