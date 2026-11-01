using Godot;

public partial class GameManager : Node
{
    private bool _isGameOver = false;

    public override void _Ready()
    {
        GD.Print("GameManager is ready!");
    }

    public void GameOver()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;

        GD.Print("Game Over!");

        var gameOverLabel = GetTree().CurrentScene.FindChild("GameOverLabel", true, false) as Label;

        if (gameOverLabel != null)
        {
            gameOverLabel.Visible = true;
        }
        else
        {
            GD.PushWarning("GameOverLabel was not found!");
        }

        GetTree().Paused = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (
            @event is InputEventKey keyEvent
            && keyEvent.Pressed
            && !keyEvent.Echo
            && keyEvent.Keycode == Key.R
        )
        {
            if (_isGameOver)
            {
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        GD.Print("Restarting game...");

        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }
}
