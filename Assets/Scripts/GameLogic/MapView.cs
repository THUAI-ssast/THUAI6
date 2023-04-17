using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using PortalPattern = System.UInt32;
public class MapView : MonoBehaviour
{
    public Tilemap tileMap;
    public TileBase roadTile;
    public TileBase obstacleTile;
    public List<TileBase> lineTiles;

    public List<Sprite> lineTextures; // 在Inspector中设置线条纹理

    void Awake()
    {
        // create line tiles
        lineTiles = new List<TileBase>();
        for(int i = 0; i < lineTextures.Count; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            // no collider
            tile.colliderType = Tile.ColliderType.None;
            tile.sprite = lineTextures[i];
            lineTiles.Add(tile);
        }
    }
    
    public void InitMap(Cell[,] map)
    {
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                if(map[i, j].isObstacle)
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), obstacleTile);
                }
                else
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                }
            }
        }
    }

    public void UpdatePortal(PortalModel portal)
    {
        int i = portal.position.x;
        int j = portal.position.y;
        TileBase tile = lineTiles[0];
        if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Left | Direction.Right | Direction.Down)) == 0) {
            tile = lineTiles[14];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Right | Direction.Down)) == 0) {
            tile = lineTiles[13];
        }
        else if ((~portal.pattern & (PortalPattern)(Direction.Left | Direction.Right | Direction.Down)) == 0) {
            tile = lineTiles[12];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Left | Direction.Down)) == 0) {
            tile = lineTiles[11];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Left | Direction.Right)) == 0) {
            tile = lineTiles[10];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Right | Direction.Down)) == 0) {
            tile = lineTiles[9];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Down)) == 0) {
            tile = lineTiles[8];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Right)) == 0) {
            tile = lineTiles[7];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Left | Direction.Down)) == 0) {
            tile = lineTiles[6];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Left | Direction.Right)) == 0) {
            tile = lineTiles[5];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up | Direction.Left)) == 0) {
            tile = lineTiles[4];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Down)) == 0) {
            tile = lineTiles[3];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Right)) == 0) {
            tile = lineTiles[2];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Up)) == 0) {
            tile = lineTiles[1];
        } else if ((~portal.pattern & (PortalPattern)(Direction.Left)) == 0) {
            tile = lineTiles[0];
        } else {
            tile = lineTiles[15];
        }
        tileMap.SetTile(new Vector3Int(i, j, 0), tile);
    }
}
