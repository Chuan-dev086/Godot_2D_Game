using System;
using Godot;

// create a player character
public partial class Player : CharacterBody2D
{
    // speed of player
    private float _speed = 300f;

    private PackedScene _bulletScene = GD.Load<PackedScene>("res://bullet.tscn");

    // will ready when player node is ready
    public override void _Ready()
    {
        // will print out when game start
        GD.Print("Player node ready!");
        QueueRedraw(); //Force a redraw of the node
    }

    // to refresh the player's status and define the player's speed
    public override void _PhysicsProcess(double delta)
    {
        // Define a move direction vector
        // Vector2 means 2D with only (X，Y) cause this is only a 2D game then .Zero means the zero point(origin)(0,0)
        Vector2 velocity = Vector2.Zero;

        // get the keyboard input use key (WASD)
        // velocity minus means move backwards
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
        }

        Velocity = velocity * _speed;
        MoveAndSlide();

        // player can't go out from screen border
        float minX = -576 + 25;
        float maxX = 576 - 25;
        float minY = -324 + 25;
        float maxY = 324 - 25;

        Position = new Vector2(
            Mathf.Clamp(Position.X, minX, maxX),
            Mathf.Clamp(Position.Y, minY, maxY)
        );
    }

    // Handle mouse click input
    public override void _Input(InputEvent @event)
    {
        // if the user left click will trigger the fire
        if (
            @event is InputEventMouseButton mouseEvent
            && mouseEvent.Pressed
            && mouseEvent.ButtonIndex == MouseButton.Left
        )
        {
            Shoot();
        }
    }

    // the logic of bullet trigger
    private void Shoot()
    {
        if (_bulletScene == null)
        {
            GD.PrintErr("Bullet scene not found. Please check the path and spelling!");
            return;
        }

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
