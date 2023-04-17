using System;

public class GameModel : Singleton<GameModel>
{
    public const float DefaultGameTime = 300.0f;
    public const int PlayerCountEachTeam = 3;
    public const int TeamCount = 2;

    public event EventHandler<int[]> ScoreChangedEvent;
    public event EventHandler<float> TimeLeftChangedEvent;

    public int[] teamScore { get; private set; }
    public float timeLeft { get; private set; }
    public int frame { get; private set; }

    private GameModel()
    {
        teamScore = new int[TeamCount]; // all 0
        // It will be overwritten by the config file.
        timeLeft = DefaultGameTime;
    }

    public void SetTimeLeft(float timeLeft)
    {
        this.timeLeft = timeLeft;
        TimeLeftChangedEvent?.Invoke(this, timeLeft);
    }

    public void UpdateTime(float deltaTime)
    {
        timeLeft -= deltaTime;
        if (timeLeft < 0.0f)
        {
            timeLeft = 0.0f;
        }
        TimeLeftChangedEvent?.Invoke(this, timeLeft);
        frame++;
    }

    public void AddScore(Team team, int score)
    {
        teamScore[(int)team] += score;
        ScoreChangedEvent?.Invoke(this, teamScore);
    }
}
