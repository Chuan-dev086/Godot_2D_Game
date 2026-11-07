using Godot;

public partial class Enemy : CharacterBody2D
{
    public float Speed = 150f;

    // Enemy Health
    private int _maxHealth = 3;
    private int _currentHealth;

    private Node2D _player;

    // --- 新增：用来引用飘字场景 ---
    [Export]
    public PackedScene DamageNumberScene;

    // ----------------------------

    public override void _Ready()
    {
        AddToGroup("enemies");

        _currentHealth = _maxHealth;

        _player = GetTree().GetFirstNodeInGroup("player") as Node2D;

        GD.Print($"Enemy spawned! HP: {_currentHealth}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(_player))
        {
            _player = GetTree().GetFirstNodeInGroup("player") as Node2D;
            if (!IsInstanceValid(_player))
                return;
        }

        Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

        Velocity = direction * Speed;
        MoveAndSlide();

        float distanceX = Mathf.Abs(GlobalPosition.X - _player.GlobalPosition.X);
        float distanceY = Mathf.Abs(GlobalPosition.Y - _player.GlobalPosition.Y);

        if (distanceX < 55f && distanceY < 55f)
        {
            if (_player is Player playerScript)
            {
                playerScript.TakeDamage();
            }

            QueueFree();
        }
    }

    // Receive damage from bullets
    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0 || damage <= 0)
            return;

        _currentHealth -= damage;
        QueueRedraw();

        GD.Print($"Enemy took {damage} damage! HP: {_currentHealth}/{_maxHealth}");

        // --- 1. 生成伤害飘字 ---
        if (DamageNumberScene != null)
        {
            var damageNum = DamageNumberScene.Instantiate<DamageNumber>();
            GetTree().CurrentScene.AddChild(damageNum);
            damageNum.ShowDamage(damage, GlobalPosition + new Vector2(0, -30));
        }

        // --- 2. 命中顿帧 (Hit Stop) ---
        Engine.TimeScale = 0.05f;
        GetTree().CreateTimer(0.05f, true, false, true).Timeout += () =>
        {
            Engine.TimeScale = 1.0f;
        };

        // --- 3. 屏幕震动 (Screen Shake) ---
        var camera = GetTree().GetFirstNodeInGroup("camera") as CameraShake;
        if (camera != null)
        {
            camera.Shake(15f, 0.3f); // 强度15，时长0.3秒
        }

        // --- 4. 死亡判定 ---
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    // Handle enemy death
    private void Die()
    {
        GD.Print("Enemy Defeated !!");

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(1);
        }
        else
        {
            var scoreManager = GetNodeOrNull<ScoreManager>("/root/ScoreManager");
            scoreManager?.AddScore(1);
        }

        QueueFree();
    }

    public override void _Draw()
    {
        // Draw Enemy
        DrawRect(new Rect2(-20, -20, 40, 40), Colors.Red);

        // Calculate Health Percentage
        float healthPercentage = (float)_currentHealth / _maxHealth;

        // Health Bar Background
        DrawRect(new Rect2(-20, -32, 40, 5), Colors.DarkGray);

        // Health Bar Foreground
        DrawRect(new Rect2(-20, -32, 40 * healthPercentage, 5), Colors.Green);
    }
}
