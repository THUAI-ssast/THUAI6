public class GameModel : Singleton<GameModel>
{
    // TODO: to be decided
    public const float GameTime = 5.0f; // seconds
    public const int PlayerCountEachTeam = 3;
    public const int TeamCount = 2;

    public int[] teamScore { get; private set; }
    public float timeLeft { get; private set; }

    private GameModel()
    {
        teamScore = new int[TeamCount]; // all 0
        timeLeft = GameTime;
    }

    public void UpdateTime(float deltaTime)
    {
        timeLeft -= deltaTime;
    }

    public void AddScore(Team team, int score)
    {
        teamScore[(int)team] += score;
    }
}
