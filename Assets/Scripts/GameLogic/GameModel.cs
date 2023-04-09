public class GameModel : Singleton<GameModel>
{
    public const float DefaultGameTime = 300.0f;
    public const int PlayerCountEachTeam = 3;
    public const int TeamCount = 2;

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
    }

    public void UpdateTime(float deltaTime)
    {
        timeLeft -= deltaTime;
        frame++;
    }

    public void AddScore(Team team, int score)
    {
        teamScore[(int)team] += score;
    }
}
