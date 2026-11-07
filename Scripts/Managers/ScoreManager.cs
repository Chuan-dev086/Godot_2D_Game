using Godot;

public partial class ScoreManager : Node
{
    public static ScoreManager Instance { get; private set; }

    [Signal]
    public delegate void ScoreChangedEventHandler(int newScore);

    public int CurrentScore { get; private set; } = 0;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void _Ready()
    {
        ResetScore();
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        EmitSignal(SignalName.ScoreChanged, CurrentScore);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        EmitSignal(SignalName.ScoreChanged, CurrentScore);
    }
}
