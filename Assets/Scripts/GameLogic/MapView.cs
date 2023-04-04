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
        if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile = lineTiles[14];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile = lineTiles[13];
        }else if(portal.pattern == (PortalPattern) ( LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile = lineTiles[12];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown & LineInPortalPattern.Down)){
            tile = lineTiles[11];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown)){
            tile = lineTiles[10];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile = lineTiles[9];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.Down)){
            tile = lineTiles[8];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.RightDown)){
            tile = lineTiles[7];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.LeftDown & LineInPortalPattern.Down)){
            tile = lineTiles[6];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown)){
            tile = lineTiles[5];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown)){
            tile = lineTiles[4];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Down)){
            tile = lineTiles[3];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.RightDown)){
            tile = lineTiles[2];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center)){
            tile = lineTiles[1];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.LeftDown)){
            tile = lineTiles[0];
        }else{
            tile = lineTiles[15];}
        tileMap.SetTile(new Vector3Int(i, j, 0), tile);
    }
}
