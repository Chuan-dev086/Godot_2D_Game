using Godot;

public partial class Enemy : CharacterBody2D
{
    public float Speed = 150f;

    // Enemy Health
    private int _maxHealth = 3;
    private int _currentHealth;

    private Node2D _player;

    public override void _Ready()
    {
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
