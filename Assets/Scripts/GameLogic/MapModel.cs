using System;
using System.Collections;
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
        // TODO: to be implemented.
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
                    this.map[i, j].isObstacle = true;
                }
            }
        }
        // initialize players(model only)
        const int teamCount = GameModel.TeamCount;
        const int playerCountEachTeam = GameModel.PlayerCountEachTeam;
        const int playerCount = teamCount * playerCountEachTeam;
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new PlayerModel(i, (Team)(i / playerCountEachTeam), GetRandomPosition()));
        }
    }

    // return a random position that is not an obstacle
    public Vector2Int GetRandomPosition()
    {
        // TODO: to be implemented
        int x = UnityEngine.Random.Range(0, map.GetLength(0));
        int y = UnityEngine.Random.Range(0, map.GetLength(1));
        if (map[x, y].isObstacle)
        {
            return GetRandomPosition(); // recursive call
        }
        return new Vector2Int(x, y);
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

    // TODO: to be implemented

    public void AddLine(Vector2Int cellPosition, LineInPortalPattern line)
    {
        List<PortalModel> portalsModified = new List<PortalModel>();
        // add the line for all related portals, by calling AddLine function for all related Portal
        // update portalClassifiedByPattern

        map[cellPosition.x, cellPosition.y].portal.AddLine(line);
        if (line == LineInPortalPattern.LeftDown)
        {
            map[cellPosition.x - 1, cellPosition.y - 1].portal.AddLine(LineInPortalPattern.RightUp);
            map[cellPosition.x - 1, cellPosition.y].portal.AddLine(LineInPortalPattern.RightDown);
            portalsModified.Add(map[cellPosition.x - 1, cellPosition.y - 1].portal);
            portalsModified.Add(map[cellPosition.x - 1, cellPosition.y].portal);
        }
        // else if (line == LineInPortalPattern.LeftUp)
        // {
        //     map[cellPosition.x - 1, cellPosition.y + 1].portal.AddLine(LineInPortalPattern.RightDown);
        //     map[cellPosition.x - 1, cellPosition.y].portal.AddLine(LineInPortalPattern.RightUp);
        //     portalsModified.Add(map[cellPosition.x - 1, cellPosition.y + 1].portal);
        //     portalsModified.Add(map[cellPosition.x - 1, cellPosition.y].portal);
        // }
        else if (line == LineInPortalPattern.RightDown)
        {
            map[cellPosition.x + 1, cellPosition.y - 1].portal.AddLine(LineInPortalPattern.LeftUp);
            map[cellPosition.x + 1, cellPosition.y].portal.AddLine(LineInPortalPattern.LeftDown);
            portalsModified.Add(map[cellPosition.x + 1, cellPosition.y - 1].portal);
            portalsModified.Add(map[cellPosition.x + 1, cellPosition.y].portal);
        }
        else if (line == LineInPortalPattern.Center)
        {
            map[cellPosition.x, cellPosition.y + 1].portal.AddLine(LineInPortalPattern.Down);
            map[cellPosition.x, cellPosition.y - 1].portal.AddLine(LineInPortalPattern.Up);
            portalsModified.Add(map[cellPosition.x, cellPosition.y + 1].portal);
            portalsModified.Add(map[cellPosition.x, cellPosition.y - 1].portal);
        }
        else if (line == LineInPortalPattern.Down)
        {
            map[cellPosition.x, cellPosition.y - 1].portal.AddLine(LineInPortalPattern.Center);
            map[cellPosition.x, cellPosition.y - 2].portal.AddLine(LineInPortalPattern.Up);
            portalsModified.Add(map[cellPosition.x, cellPosition.y - 1].portal);
            portalsModified.Add(map[cellPosition.x, cellPosition.y - 2].portal);
        }

        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

    public void RemoveLine(Vector2Int cellPosition, LineInPortalPattern line)
    {
        // TODO: similar to AddLine
        List<PortalModel> portalsModified = new List<PortalModel>();

        map[cellPosition.x, cellPosition.y].portal.RemoveLine(line);
        if (line == LineInPortalPattern.LeftDown)
        {
            map[cellPosition.x - 1, cellPosition.y - 1].portal.RemoveLine(LineInPortalPattern.RightUp);
            map[cellPosition.x - 1, cellPosition.y].portal.RemoveLine(LineInPortalPattern.RightDown);
            portalsModified.Add(map[cellPosition.x - 1, cellPosition.y - 1].portal);
            portalsModified.Add(map[cellPosition.x - 1, cellPosition.y].portal);
        }
        else if (line == LineInPortalPattern.RightDown)
        {
            map[cellPosition.x + 1, cellPosition.y - 1].portal.RemoveLine(LineInPortalPattern.LeftUp);
            map[cellPosition.x + 1, cellPosition.y].portal.RemoveLine(LineInPortalPattern.LeftDown);
            portalsModified.Add(map[cellPosition.x + 1, cellPosition.y - 1].portal);
            portalsModified.Add(map[cellPosition.x + 1, cellPosition.y].portal);
        }
        else if (line == LineInPortalPattern.Center)
        {
            map[cellPosition.x, cellPosition.y + 1].portal.RemoveLine(LineInPortalPattern.Down);
            map[cellPosition.x, cellPosition.y - 1].portal.RemoveLine(LineInPortalPattern.Up);
            portalsModified.Add(map[cellPosition.x, cellPosition.y + 1].portal);
            portalsModified.Add(map[cellPosition.x, cellPosition.y - 1].portal);
        }else if (line == LineInPortalPattern.Down)
        {
            map[cellPosition.x, cellPosition.y - 1].portal.RemoveLine(LineInPortalPattern.Center);
            map[cellPosition.x, cellPosition.y - 2].portal.RemoveLine(LineInPortalPattern.Up);
            portalsModified.Add(map[cellPosition.x, cellPosition.y - 1].portal);
            portalsModified.Add(map[cellPosition.x, cellPosition.y - 2].portal);
        }
        
        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

}
