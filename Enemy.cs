using System;
using Godot;

public partial class Enemy : CharacterBody2D // 变成实体
{
    public float Speed = 150f;
    private Node2D _player;

    public override void _Ready()
    {
        _player = GetTree().Root.GetNode<Node2D>("Node2D/Player");
    }

    // 加一个布尔值，防止多个怪物同时撞你导致疯狂报错
    private bool _hasTriggeredGameOver = false;

    public override void _PhysicsProcess(double delta)
    {
        // 如果已经触发过死亡了，就不再执行后面的代码
        if (_hasTriggeredGameOver || !IsInstanceValid(_player))
            return;

        Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();

        float distanceX = Mathf.Abs(GlobalPosition.X - _player.GlobalPosition.X);
        float distanceY = Mathf.Abs(GlobalPosition.Y - _player.GlobalPosition.Y);

        if (distanceX < 55f && distanceY < 55f)
        {
            // 如果是玩家（Player.cs）类型，就调用它的受伤方法
            if (_player is Player playerScript)
            {
                playerScript.TakeDamage();
            }

            // 怪物撞到人后自己也消失（相当于自爆）
            QueueFree();
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
