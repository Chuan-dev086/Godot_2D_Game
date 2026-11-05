using System;
using Godot;

public partial class CameraShake : Camera2D
{
    private float _shakeIntensity = 0f; // 当前震动强度
    private float _maxIntensity = 0f; // 初始最大强度
    private float _shakeTimer = 0f; // 剩余时间
    private float _duration = 0f; // 总持续时间
    private Vector2 _originalOffset; // 原始偏移

    public override void _Ready()
    {
        _originalOffset = Offset;

        // 关键修改：即使游戏时间变慢（顿帧），相机也要一直更新！
        ProcessMode = ProcessModeEnum.Always;
    }

    public void Shake(float intensity = 12f, float duration = 0.25f)
    {
        _maxIntensity = intensity;
        _shakeIntensity = intensity;
        _shakeTimer = duration;
        _duration = duration;
    }

    public override void _Process(double delta)
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= (float)delta;

            // 计算当前强度（线性衰减：从最大强度逐渐降到0）
            float progress = _shakeTimer / _duration;
            _shakeIntensity = _maxIntensity * progress;

            // 随机生成偏移
            float randomX = (float)GD.RandRange(-_shakeIntensity, _shakeIntensity);
            float randomY = (float)GD.RandRange(-_shakeIntensity, _shakeIntensity);
            Offset = _originalOffset + new Vector2(randomX, randomY);
        }
        else if (Offset != _originalOffset)
        {
            // 结束震动，恢复原位
            Offset = _originalOffset;
        }
    }
}
