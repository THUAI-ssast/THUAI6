using System;
using System.Collections.Generic;
using UnityEngine;

using PortalPattern = System.UInt32;

public struct Cell
{
    public bool isObstacle;
    public PortalModel portal;
    public List<BombModel> bombs;

    public Cell(bool isObstacle = false)
    {
        this.isObstacle = isObstacle;
        this.portal = null;
        this.bombs = null;
    }
}

public class MapModel : Singleton<MapModel>
{
    public const int Width = 20;
    public const int Height = 20;

    public event EventHandler<List<PortalModel>> PortalLineModifiedEvent;

    public Cell[,] map = new Cell[Width, Height];
    public List<PlayerModel> players { get; private set; } = new List<PlayerModel>();
    public List<BombModel> bombs { get; private set; } = new List<BombModel>();
    public Dictionary<PortalPattern, List<PortalModel>> portalsClassifiedByPattern { get; private set; } = new Dictionary<PortalPattern, List<PortalModel>>();

    private System.Random _processRandom;
    private int _processSeed;
    public int GetProcessSeed()
    {
        return _processSeed;
    }

    static int[,] CreateMap(int width, int height, int obstaclePercentage)
    {
        var random = new System.Random();

        int[,] map = new int[width, height];

        // Set border to obstacles
        for (int i = 0; i < width; i++)
        {
            map[i, 0] = 1;
            map[i, height - 1] = 1;
        }
        for (int j = 0; j < height; j++)
        {
            map[0, j] = 1;
            map[width - 1, j] = 1;
        }

        // Generate random obstacles in the map
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                int randValue = random.Next(0, 100);
                if (randValue < obstaclePercentage)
                {
                    map[j, i] = 1;
                }
                else
                {
                    map[j, i] = 0;
                }
            }
        }

        // Ensure the generated map is connected by setting the top-left and bottom-right cells as open
        map[1, 1] = 0;
        map[width - 2, height - 2] = 0;

        return map;
    }

    private MapModel()
    {
        // initialize random seed
        int initSeed = Guid.NewGuid().GetHashCode();
        System.Random initRandom = new System.Random(initSeed);

        // initialize map
        int[,] initialMap = CreateMap(Width, Height, 10);
        InitMap(initialMap);

        // initialize players(model only)
        const int teamCount = GameModel.TeamCount;
        const int playerCountEachTeam = GameModel.PlayerCountEachTeam;
        const int playerCount = teamCount * playerCountEachTeam;
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new PlayerModel(i, (Team)(i / playerCountEachTeam), PlayerModel.GetPositionFromCellPosition(GetRandomPosition(initRandom))));
        }

        // initialize process random seed
        _processSeed = Guid.NewGuid().GetHashCode();
        _processRandom = new System.Random(_processSeed);
    }

    public void InitMap(int[,] initialMap)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (initialMap[i, j] == 1)
                {
                    map[i, j].isObstacle = true;
                }
                else
                {
                    map[i, j].isObstacle = false;
                    map[i, j].portal = new PortalModel(new Vector2Int(i, j));
                    map[i, j].bombs = new List<BombModel>();
                }
            }
        }
    }

    public void CustomInit(int processSeed, int[,] initMapData, List<object> initPlayersData)
    {
        // Restore process random seed
        _processSeed = processSeed;
        _processRandom = new System.Random(_processSeed);

        // Restore map
        // clear map
        map = new Cell[Width, Height];
        InitMap((int[,])initMapData);

        // Restore player states
        for (int i = 0; i < initPlayersData.Count; i++)
        {
            dynamic initPlayerData = initPlayersData[i];
            PlayerModel playerModel = players[(int)initPlayerData.id];

            playerModel.SetPosition(new Vector2((float)initPlayerData.position[0], (float)initPlayerData.position[1]));
            playerModel.SetRotation((float)initPlayerData.rotation);
        }
    }

    // return a random position that is not an obstacle
    public Vector2Int GetRandomPosition(System.Random random = null)
    {
        // default random is process random
        if (random == null)
        {
            random = _processRandom;
        }
        do
        {
            int x = random.Next(0, map.GetLength(0));
            int y = random.Next(0, map.GetLength(1));
            if (!map[x, y].isObstacle)
            {
                return new Vector2Int(x, y);
            }
        } while (true);
    }

    public bool IsInMap(Vector2Int cellPosition)
    {
        return cellPosition.x >= 0 && cellPosition.x < map.GetLength(0) && cellPosition.y >= 0 && cellPosition.y < map.GetLength(1);
    }

    public bool IsRoad(Vector2Int cellPosition)
    {
        return IsInMap(cellPosition) && !map[cellPosition.x, cellPosition.y].isObstacle;
    }

    public bool CanModifyPortalLine(Vector2Int cellPosition, Direction direction)
    {
        if (!IsRoad(cellPosition))
        {
            return false;
        }
        switch (direction)
        {
            case Direction.Up:
                return IsRoad(cellPosition + Vector2Int.up);
            case Direction.Down:
                return IsRoad(cellPosition + Vector2Int.down);
            case Direction.Left:
                return IsRoad(cellPosition + Vector2Int.left);
            case Direction.Right:
                return IsRoad(cellPosition + Vector2Int.right);
            default:
                return false;
        }
    }

    public bool CanActivatePortal(Vector2Int cellPosition, Vector2Int destination)
    {
        if (!(IsRoad(cellPosition) && IsRoad(destination)))
        {
            return false;
        }

        PortalModel portal1 = map[cellPosition.x, cellPosition.y].portal;
        // Check the origin portal
        if (portal1.pattern == 0 || portal1.isActivated)
        {
            return false;
        }
        PortalModel portal2 = map[destination.x, destination.y].portal;
        // pattern of two portals must be the same
        return portal1.pattern == portal2.pattern;
    }

    public void PlaceBomb(BombModel bomb)
    {
        bombs.Add(bomb);
        map[bomb.position.x, bomb.position.y].bombs.Add(bomb);
    }

    public void RemoveBomb(BombModel bomb)
    {
        bombs.Remove(bomb);
        map[bomb.position.x, bomb.position.y].bombs.Remove(bomb);
    }

    public void SetBombPosition(BombModel bomb, Vector2Int newPosition)
    {
        map[bomb.position.x, bomb.position.y].bombs.Remove(bomb);
        map[newPosition.x, newPosition.y].bombs.Add(bomb);
    }

    public void AddLine(Vector2Int cellPosition, Direction direction)
    {
        if (!CanModifyPortalLine(cellPosition, direction))
        {
            return;
        }

        List<PortalModel> portalsModified = new List<PortalModel>();

        Action<Cell, LineInPortalPattern> AddLineForCell = (cell, line) =>
        {
            if (cell.portal == null)
            {
                return;
            }
            PortalModel portal = cell.portal;
            PortalPattern oldPattern = portal.pattern;
            if (oldPattern != 0)
            {
                portalsClassifiedByPattern[oldPattern].Remove(portal);
            }
            portal.AddLine(line);
            PortalPattern newPattern = portal.pattern;
            if (!portalsClassifiedByPattern.ContainsKey(newPattern))
            {
                portalsClassifiedByPattern.Add(newPattern, new List<PortalModel>());
            }
            portalsClassifiedByPattern[newPattern].Add(portal);
            portalsModified.Add(portal);
        };

        // add the line for all related portals
        switch (direction)
        {
            case Direction.Up:
                AddLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.Center);
                AddLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.Up);
                AddLineForCell(map[cellPosition.x, cellPosition.y + 1], LineInPortalPattern.Down);
                break;
            case Direction.Down:
                AddLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.Down);
                AddLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.Center);
                AddLineForCell(map[cellPosition.x, cellPosition.y - 2], LineInPortalPattern.Up);
                break;
            case Direction.Left:
                AddLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.LeftDown);
                AddLineForCell(map[cellPosition.x - 1, cellPosition.y], LineInPortalPattern.RightDown);
                AddLineForCell(map[cellPosition.x - 1, cellPosition.y - 1], LineInPortalPattern.RightUp);
                AddLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.LeftUp);
                break;
            case Direction.Right:
                AddLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.RightDown);
                AddLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.RightUp);
                AddLineForCell(map[cellPosition.x + 1, cellPosition.y - 1], LineInPortalPattern.LeftUp);
                AddLineForCell(map[cellPosition.x + 1, cellPosition.y], LineInPortalPattern.LeftDown);
                break;
        }

        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

    public void RemoveLine(Vector2Int cellPosition, Direction direction)
    {
        if (!CanModifyPortalLine(cellPosition, direction))
        {
            return;
        }

        List<PortalModel> portalsModified = new List<PortalModel>();

        Action<Cell, LineInPortalPattern> RemoveLineForCell = (cell, line) =>
        {
            if (cell.portal == null)
            {
                return;
            }
            PortalModel portal = cell.portal;
            PortalPattern oldPattern = portal.pattern;
            portalsClassifiedByPattern[oldPattern].Remove(portal);
            portal.RemoveLine(line);
            PortalPattern newPattern = portal.pattern;
            if (newPattern != 0)
            {
                if (!portalsClassifiedByPattern.ContainsKey(newPattern))
                {
                    portalsClassifiedByPattern.Add(newPattern, new List<PortalModel>());
                }
                portalsClassifiedByPattern[newPattern].Add(portal);
            }
            portalsModified.Add(portal);
        };

        // remove the line for all related portals
        switch (direction)
        {
            case Direction.Up:
                RemoveLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.Center);
                RemoveLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.Up);
                RemoveLineForCell(map[cellPosition.x, cellPosition.y + 1], LineInPortalPattern.Down);
                break;
            case Direction.Down:
                RemoveLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.Down);
                RemoveLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.Center);
                RemoveLineForCell(map[cellPosition.x, cellPosition.y - 2], LineInPortalPattern.Up);
                break;
            case Direction.Left:
                RemoveLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.LeftDown);
                RemoveLineForCell(map[cellPosition.x - 1, cellPosition.y], LineInPortalPattern.RightDown);
                RemoveLineForCell(map[cellPosition.x - 1, cellPosition.y - 1], LineInPortalPattern.RightUp);
                RemoveLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.LeftUp);
                break;
            case Direction.Right:
                RemoveLineForCell(map[cellPosition.x, cellPosition.y], LineInPortalPattern.RightDown);
                RemoveLineForCell(map[cellPosition.x, cellPosition.y - 1], LineInPortalPattern.RightUp);
                RemoveLineForCell(map[cellPosition.x + 1, cellPosition.y - 1], LineInPortalPattern.LeftUp);
                RemoveLineForCell(map[cellPosition.x + 1, cellPosition.y], LineInPortalPattern.LeftDown);
                break;
        }

        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

}
