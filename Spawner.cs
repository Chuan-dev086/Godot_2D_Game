using System;
using Godot;

public partial class Spawner : Timer
{
    private PackedScene _enemyScene = GD.Load<PackedScene>("res://enemy.tscn");
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    [Export]
    public float DifficultyCurve { get; set; } = 0.95f;

    [Export]
    public float MinWaitTime { get; set; } = 0.5f;

    [Export]
    public int MaxEnemies { get; set; } = 50;

    public override void _Ready()
    {
        _rng.Randomize();
        Timeout += SpawnEnemy;
    }

    private void SpawnEnemy()
    {
        if (GetTree().Paused)
            return;
        if (_enemyScene == null)
            return;

        // 检查场上敌人总数，防止内存和性能溢出
        int currentEnemies = GetTree().GetNodesInGroup("enemies").Count;
        if (currentEnemies >= MaxEnemies)
            return;

        // 1. 找到玩家当前的位置
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (!GodotObject.IsInstanceValid(player))
            return;

        var enemy = _enemyScene.Instantiate<Enemy>();

        // 2. 随机生成一个角度
        float angle = _rng.RandfRange(0, Mathf.Pi * 2);

        // 3. 距离玩家 800 像素的圆圈外生成（保证在屏幕外面一点点）
        float spawnRadius = 800f;
        Vector2 spawnOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        // 4. 怪物位置 = 玩家位置 + 随机偏移
        enemy.GlobalPosition = player.GlobalPosition + spawnOffset;

        GetParent().AddChild(enemy);

        // 5. 动态难度曲线推进：加快生成间隔，但不低于 MinWaitTime
        WaitTime = Mathf.Max(WaitTime * DifficultyCurve, MinWaitTime);
    }
}
