using System;
using Godot;

public partial class DamageNumber : Label
{
    public void ShowDamage(float amount, Vector2 position)
    {
        Text = Mathf.Round(amount).ToString();

        GlobalPosition = position;

        Tween tween = CreateTween();
        tween.SetParallel(true);

        tween
            .TweenProperty(this, "global_position:y", GlobalPosition.Y - 50, 0.6f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(this, "modulate:a", 0.0f, 0.6f).SetDelay(0.3f);

        tween.Chain().TweenCallback(Callable.From(() => QueueFree()));
    }
}
