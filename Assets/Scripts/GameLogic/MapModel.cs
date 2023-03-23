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

        // initialize players(model only)
        // get TeamCount, PlayerCount from GameModel.Instance
    }

    // return a random position that is not an obstacle
    public Vector2Int GetRandomPosition()
    {
        // TODO: to be implemented
        return new Vector2Int(0, 0);
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


        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

    public void RemoveLine(Vector2Int cellPosition, LineInPortalPattern line)
    {
        // TODO: similar to AddLine
        List<PortalModel> portalsModified = new List<PortalModel>();

        PortalLineModifiedEvent?.Invoke(this, portalsModified);
    }

}
