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

        _player = GetTree().Root.GetNode<Node2D>("Node2D/Player");

        GD.Print($"Enemy spawned! HP: {_currentHealth}");
    }

    private bool _hasTriggeredGameOver = false;

    public override void _PhysicsProcess(double delta)
    {
        if (_hasTriggeredGameOver || !IsInstanceValid(_player))
            return;

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

        // --- 新增：生成伤害飘字 ---
        if (DamageNumberScene != null)
        {
            // 1. 实例化飘字场景
            var damageNum = DamageNumberScene.Instantiate<DamageNumber>();

            // 2. 把飘字加到当前场景树中，而不是敌人节点下
            // 这样即使敌人马上要死了（QueueFree），飘字也能播完动画
            GetTree().CurrentScene.AddChild(damageNum);

            // 3. 传入伤害值和位置（向上偏移 30 像素，避免遮挡敌人血条）
            damageNum.ShowDamage(damage, GlobalPosition + new Vector2(0, -30));
        }
        // --------------------------

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    // Handle enemy death
    private void Die()
    {
        GD.Print("Enemy Defeated !!");

        var scoreLabel = GetTree().Root.GetNode<Label>("Node2D/CanvasLayer/ScoreLabel");

        string currentText = scoreLabel.Text;
        int currentScore = int.Parse(currentText.Split(':')[1].Trim());

        currentScore += 1;
        scoreLabel.Text = $"Score: {currentScore}";

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
