using System;
using Godot;

public partial class Enemy : Area2D
{
    public float Speed = 150f;
    private Node2D _player;

    public override void _Ready()
    {
        _player = GetTree().Root.GetNode<Node2D>("Node2D/Player");

        AreaEntered += OnAreaEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(_player))
        {
            Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
            Position += direction * Speed * (float)delta;
        }
    }

    // the method that player meet enemy
    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent().Name == "Player" || area.Name == "Player")
        {
            GD.Print("Eliminated by monster! GAME OVER!!!");

            var gameOverLabel = GetTree().Root.GetNode<Label>("Node2D/CanvasLayer/GameOverLabel");
            gameOverLabel.Visible = true;

            area.GetParent().QueueFree();

            GetTree().Paused = true;
        }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(-20, -20, 40, 40), Colors.Red);
    }

    public void TakeDamage()
    {
        GD.Print("Enemy Defeated !!");

        var scoreLabel = GetTree().Root.GetNode<Label>("Node2D/CanvasLayer/ScoreLabel");

        string currentText = scoreLabel.Text;
        int currentScore = int.Parse(currentText.Split(':')[1].Trim());

        currentScore += 1;
        scoreLabel.Text = $"Score: {currentScore}";

        QueueFree();
    }
}
