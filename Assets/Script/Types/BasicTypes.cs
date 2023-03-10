using PortalPattern = System.UInt32;

public enum LineInPortalPattern
{
    // sorted by stroke order of Chinese character `日`
    LeftUp = 1,
    LeftDown = 2,
    Up = 4,
    RightUp = 8,
    RightDown = 16,
    Center = 32,
    Down= 64
}

public enum Team
{
    Red,
    Blue
}

public struct PlayerState {
    bool isAlive;
    bool isShooting;
    bool isChangingBullet;
    bool isPlacingBomb;
    bool isModifyingPortal;
    bool isUsingPortal;

    bool canShoot;
    bool canChangeBullet;
    bool canPlaceBomb;
    bool canModifyPortal;
    bool canUsePortal;
}

public struct BlockState
{
    bool isObstacle;
    Portal portal;
    Bomb bomb;
}

public struct Coordinate
{
    int x, y;
}
