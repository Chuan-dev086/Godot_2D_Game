using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Signal]
    public delegate void GameOverTriggeredEventHandler();

    private bool _isGameOver = false;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void _Ready()
    {
        GD.Print("GameManager is ready!");
        ProcessMode = ProcessModeEnum.Always;

        // Connect to ScoreManager to update ScoreLabel in scene if present
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged += OnScoreChanged;
            OnScoreChanged(ScoreManager.Instance.CurrentScore);
        }
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged -= OnScoreChanged;
        }
    }

    private void OnScoreChanged(int newScore)
    {
        var scoreLabel = GetTree().CurrentScene?.FindChild("ScoreLabel", true, false) as Label;
        if (scoreLabel != null)
        {
            scoreLabel.Text = $"Score: {newScore}";
        }
    }

    public void GameOver()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;
        GD.Print("Game Over!");

        EmitSignal(SignalName.GameOverTriggered);

        var gameOverLabel = GetTree().CurrentScene?.FindChild("GameOverLabel", true, false) as Label;
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

    public void RestartGame()
    {
        GD.Print("Restarting game...");

        _isGameOver = false;
        GetTree().Paused = false;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        GetTree().ReloadCurrentScene();
    }
}
