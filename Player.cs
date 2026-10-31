using System;
using Godot;

public partial class Player : CharacterBody2D
{
    private float _speed = 300f;
    private PackedScene _bulletScene = GD.Load<PackedScene>("res://bullet.tscn");

    // 新增：生命值与状态
    private int _maxHealth = 3;
    private int _currentHealth;
    private bool _isInvincible = false;

    public override void _Ready()
    {
        _currentHealth = _maxHealth;
        UpdateHealthUI(); // 更新 UI
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

        var cam = GetParent().GetNode<Camera2D>("Camera2D");
        if (cam != null)
            cam.GlobalPosition = GlobalPosition;
    }

    // 受伤方法（供怪物调用）
    public void TakeDamage()
    {
        if (_isInvincible)
            return; // 如果在无敌状态，直接无视伤害

        _currentHealth -= 1;
        GD.Print($"玩家受伤！剩余生命: {_currentHealth}");
        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            GameOver();
        }
        else
        {
            // 开启无敌帧（1秒）
            _isInvincible = true;
            GetTree().CreateTimer(1.0).Timeout += () => _isInvincible = false;

            // 受伤闪烁特效（可选，让你视觉上知道自己无敌了）
            Modulate = new Color(1, 0.5f, 0.5f); // 变红
            GetTree().CreateTimer(0.2).Timeout += () => Modulate = Colors.White;
        }
    }

    private void UpdateHealthUI()
    {
        var hpLabel = GetTree().Root.FindChild("HealthLabel", true, false) as Label;
        if (hpLabel != null)
            hpLabel.Text = $"HP: {_currentHealth}";
    }

    private void GameOver()
    {
        var gameOverLabel = GetTree().Root.FindChild("GameOverLabel", true, false) as Label;
        if (gameOverLabel != null)
            gameOverLabel.Visible = true;

        QueueFree();
        GetTree().Paused = true;
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
        // 稍微改变一下颜色，如果受伤了可以给个红白交替，这里保持原样
        DrawRect(new Rect2(-25, -25, 50, 50), Colors.Cyan);
    }
}
