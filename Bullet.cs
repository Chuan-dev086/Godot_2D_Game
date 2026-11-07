using System;
using Godot;

public partial class Bullet : Area2D
{
    // the speed of bullet
    public float Speed = 800f;

    // the flying direction of bullet
    public Vector2 Direction = Vector2.Zero;

    // Bullet damage
    public float Damage { get; set; } = 1f;

    // Bullet lifetime control
    private float _lifetime = 2.0f;
    private float _timer = 0f;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        // let bullet keep flying with the setting direction
        Position += Direction * Speed * (float)delta;

        _timer += (float)delta;
        if (_timer > _lifetime)
        {
            QueueFree();
        }
    }

    public override void _Draw()
    {
        // Draw a radius 5 and yellow circle to represent bullet
        DrawCircle(Vector2.Zero, 5f, Colors.Yellow);
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("player") || area.IsInGroup("player_hurtbox") || area.GetParent()?.IsInGroup("player") == true)
            return;

        GD.Print("The bullet hit something");
        QueueFree();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player") || body.IsInGroup("player_hurtbox"))
            return;

        if (body is Enemy enemy)
        {
            enemy.TakeDamage((int)Damage);
            QueueFree();
            return;
        }

        GD.Print("The bullet hit a solid wall.");
        QueueFree();
    }
}
