using System;
using System.Collections.Generic;
using UnityEngine;

using PortalPattern = System.UInt32;

public struct Cell
{
    public bool isObstacle;
    public PortalModel portal;
    public List<BombModel> bombs;

    public Cell(bool isObstacle, PortalModel portal = null, List<BombModel> bombs = null)
    {
        this.isObstacle = isObstacle;
        this.portal = portal;
        this.bombs = bombs;
    }
}

public class MapModel : Singleton<MapModel>
{
    // TODO: to be decided
    public const int Width = 20;
    public const int Height = 20;

    public event EventHandler<List<PortalModel>> PortalLineModifiedEvent;

    public Cell[,] map = new Cell[Width, Height];
    public List<PlayerModel> players { get; private set; } = new List<PlayerModel>();
    public List<BombModel> bombs { get; private set; } = new List<BombModel>();
    public Dictionary<PortalPattern, List<PortalModel>> portalsClassifiedByPattern { get; private set; } = new Dictionary<PortalPattern, List<PortalModel>>();

    public MapModel()
    {
        // initialize map
        int[,] initialMap = new int[,]
        {
{0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0},
{0,0,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0},
{1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1,1},
{0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,1,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0},
{0,0,0,0,1,0,0,1,1,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0},
{0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,0,0,0,0,0},
{0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,1,0,0},
{0,1,0,0,0,1,0,0,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,1,0,1,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0},
{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,0,1,0,1,0,0,0,0,0,0,1,0,1,0,0,1,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,1,1,1,0,0,1,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
{0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,1,0,0,1,1,0,1,1,0,0},
{0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0},
{0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,1,0},
{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1},
{0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0},
{0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
{1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
{0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0},
{0,1,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
{0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
{0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0,0,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0},
{0,0,1,0,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0},
{0,0,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
{0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0}
        };
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
                    map[i, j].portal = new PortalModel(new Vector2Int(i, j));
                }
            }
        }
        // initialize players(model only)
        const int teamCount = GameModel.TeamCount;
        const int playerCountEachTeam = GameModel.PlayerCountEachTeam;
        const int playerCount = teamCount * playerCountEachTeam;
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new PlayerModel(i, (Team)(i / playerCountEachTeam), PlayerModel.GetPositionFromCellPosition(GetRandomPosition())));
        }
    }

    // return a random position that is not an obstacle
    public Vector2Int GetRandomPosition()
    {
        do
        {
            int x = UnityEngine.Random.Range(0, map.GetLength(0));
            int y = UnityEngine.Random.Range(0, map.GetLength(1));
            if (!map[x, y].isObstacle)
            {
                return new Vector2Int(x, y);
            }
        } while (true);
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

    public void AddLine(Vector2Int cellPosition, Direction direction)
    {
        List<PortalModel> portalsModified = new List<PortalModel>();

        Action<PortalModel, LineInPortalPattern> AddLineForPortal = (portal, line) =>
        {
            portalsClassifiedByPattern[portal.pattern].Remove(portal);
            portal.AddLine(line);
            portalsClassifiedByPattern[portal.pattern].Add(portal);
            portalsModified.Add(portal);
        };

        // add the line for all related portals
        AddLineForPortal(map[cellPosition.x, cellPosition.y].portal, (LineInPortalPattern)direction);
        switch (direction)
        {
            case Direction.Up:
                int currentX = cellPosition.x;
                int currentY = cellPosition.y + 1;
                if (currentY < map.GetLength(1) && map[currentX, currentY].portal != null)
                {
                    AddLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Down);
                }
                break;
            case Direction.Down:
                currentX = cellPosition.x;
                currentY = cellPosition.y - 1;
                if (currentY >= 0 && map[currentX, currentY].portal != null)
                {
                    AddLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Up);
                }
                break;
            case Direction.Left:
                currentX = cellPosition.x - 1;
                currentY = cellPosition.y;
                if (currentX >= 0 && map[currentX, currentY].portal != null)
                {
                    AddLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Right);
                }
                break;
            case Direction.Right:
                currentX = cellPosition.x + 1;
                currentY = cellPosition.y;
                if (currentX < map.GetLength(0) && map[currentX, currentY].portal != null)
                {
                    AddLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Left);
                }
                break;
        }

        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

    public void RemoveLine(Vector2Int cellPosition, Direction direction)
    {
        List<PortalModel> portalsModified = new List<PortalModel>();

        Action<PortalModel, LineInPortalPattern> RemoveLineForPortal = (portal, line) =>
        {
            portalsClassifiedByPattern[portal.pattern].Remove(portal);
            portal.RemoveLine(line);
            portalsClassifiedByPattern[portal.pattern].Add(portal);
            portalsModified.Add(portal);
        };
        
        // remove the line for all related portals
        RemoveLineForPortal(map[cellPosition.x, cellPosition.y].portal, (LineInPortalPattern)direction);
        switch (direction)
        {
            case Direction.Up:
                int currentX = cellPosition.x;
                int currentY = cellPosition.y + 1;
                if (currentY < map.GetLength(1) && map[currentX, currentY].portal != null)
                {
                    RemoveLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Down);
                }
                break;
            case Direction.Down:
                currentX = cellPosition.x;
                currentY = cellPosition.y - 1;
                if (currentY >= 0 && map[currentX, currentY].portal != null)
                {
                    RemoveLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Up);
                }
                break;
            case Direction.Left:
                currentX = cellPosition.x - 1;
                currentY = cellPosition.y;
                if (currentX >= 0 && map[currentX, currentY].portal != null)
                {
                    RemoveLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Right);
                }
                break;
            case Direction.Right:
                currentX = cellPosition.x + 1;
                currentY = cellPosition.y;
                if (currentX < map.GetLength(0) && map[currentX, currentY].portal != null)
                {
                    RemoveLineForPortal(map[currentX, currentY].portal, (LineInPortalPattern)Direction.Left);
                }
                break;
        }
        
        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

}
