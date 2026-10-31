using System;
using Godot;

public partial class Player : Node2D
{
    // speed of player
    private float _speed = 300f;

    public override void _Ready()
    {
        // will print out when game start
        GD.Print("Player node ready!");
    }

    public override void _Process(double delta)
    {
        // Define a move direction vector
        Vector2 velocity = Vector2.Zero;

        // get the keyboard input use key (WASD)
        if (Input.IsKeyPressed(Key.W))
            velocity.Y -= 1;
        if (Input.IsKeyPressed(Key.S))
            velocity.Y += 1;
        if (Input.IsKeyPressed(Key.A))
            velocity.X -= 1;
        if (Input.IsKeyPressed(Key.D))
            velocity.X += 1;

        // it will move when you press direction key
        if (velocity != Vector2.Zero)
        {
            // Normalized() ensures diagonal movement isn't faster than straight movement.
            velocity = velocity.Normalized();

            // it will renew the position of node
            Position += velocity * _speed * (float)delta;
        }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(0, 0, 50, 50), Colors.Cyan);
    }
}
