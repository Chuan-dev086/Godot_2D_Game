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
        if (GetTree().Paused)
            return;
        if (_enemyScene == null)
            return;

        // 1. 找到玩家当前的位置
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (!GodotObject.IsInstanceValid(player))
            return;

        var enemy = _enemyScene.Instantiate<Enemy>();

        // 2. 随机生成一个角度
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        float angle = rng.RandfRange(0, Mathf.Pi * 2);

        // 3. 距离玩家 800 像素的圆圈外生成（保证在屏幕外面一点点）
        float spawnRadius = 800f;
        Vector2 spawnOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        // 4. 怪物位置 = 玩家位置 + 随机偏移
        enemy.GlobalPosition = player.GlobalPosition + spawnOffset;

        GetParent().AddChild(enemy);
    }
}
