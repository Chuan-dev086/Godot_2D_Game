using System;
using Godot;

public partial class Bullet : Area2D
{
    // the spped of bullet
    public float Speed = 800f;

    // the flying direction of bullet
    public Vector2 Direction = Vector2.Zero;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        // let bullet keep flying with the setting direction
        Position += Direction * Speed * (float)delta;
    }

    public override void _Draw()
    {
        // Draw a radius 5 and yellow circle to represent bullet
        DrawCircle(Vector2.Zero, 5f, Colors.Yellow);
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent().Name == "Player" || area.Name == "Player")
            return;

        GD.Print("The bullet hit something");
        QueueFree();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.Name == "Player")
            return;

        if (body is Enemy enemy)
        {
            enemy.TakeDamage(1);
            QueueFree();
            return;
        }

        GD.Print("The bullet hit a solid wall.");
        QueueFree();
    }
}
