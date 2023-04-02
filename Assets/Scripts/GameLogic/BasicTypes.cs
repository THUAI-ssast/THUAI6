public enum Team
{
    Red,
    Blue
}

public static class TeamExtensions
{
    public static Team GetOppositeTeam(this Team team)
    {
        return team == Team.Red ? Team.Blue : Team.Red;
    }
}

public enum ForwardOrBackward
{
    Forward,
    Backward
}

public enum LeftOrRight
{
    Left,
    Right
}

public enum LineInPortalPattern
{
    // sorted by stroke order of Chinese character `日`
    LeftUp = 1,
    LeftDown = 2,
    Up = 4,
    RightUp = 8,
    RightDown = 16,
    Center = 32,
    Down = 64
}

public enum Direction
{
    Up = LineInPortalPattern.Center,
    Down = LineInPortalPattern.Down,
    Left = LineInPortalPattern.LeftDown,
    Right = LineInPortalPattern.RightDown,
}