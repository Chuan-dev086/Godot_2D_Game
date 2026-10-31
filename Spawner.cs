using System;
using Godot;

public partial class Spawner : Timer
{
    private PackedScene _enemyScene = GD.Load<PackedScene>("res://enemy.tscn");

    public override void _Ready()
    {
        Timeout += SpawnEnemy;
    }

    private void SpawnEnemy()
    {
        if (_enemyScene == null)
            return;

        var enemy = _enemyScene.Instantiate<Enemy>();

        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        int side = rng.RandiRange(0, 3);

        float minX = -600,
            maxX = 600,
            minY = -350,
            maxY = 350;
        Vector2 spawnPos = Vector2.Zero;

        switch (side)
        {
            case 0:
                spawnPos = new Vector2(rng.RandfRange(minX, maxX), minY);
                break;
            case 1:
                spawnPos = new Vector2(rng.RandfRange(minX, maxX), maxY);
                break;
            case 2:
                spawnPos = new Vector2(minX, rng.RandfRange(minY, maxY));
                break;
            case 3:
                spawnPos = new Vector2(maxX, rng.RandfRange(minY, maxY));
                break;
        }

        enemy.GlobalPosition = spawnPos;

        GetParent().AddChild(enemy);
    }
}
