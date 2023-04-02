using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using PortalPattern = System.UInt32;
public class MapView : MonoBehaviour
{
    public Tilemap tileMap;
    public TileBase roadTile;
    public TileBase obstacleTile;
    // public Sprite lineTexture1; 
    public List<Sprite> lineTextures; // 在Inspector中设置线条纹理

    
    public void InitMap(Cell[,] map)
    {
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                if(map[i, j].isObstacle)
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), obstacleTile);
                    // 设置障碍物的碰撞体
                    BoxCollider2D collider = tileMap.gameObject.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(1, 1); // 设置碰撞体的大小
                    collider.offset = new Vector2(0.5f, 0.5f); // 设置碰撞体的偏移量
                }
                else
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), roadTile);
                    // Tile tile = ScriptableObject.CreateInstance<Tile>();
                    // tile.sprite = lineTextures[0];
                    // tileMap.SetTile(new Vector3Int(i, j, 0), tile);
                }
            }
        }
    }

    public void UpdatePortal(PortalModel portal)
    {
        int i = portal.position.x;
        int j = portal.position.y;
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[14];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[13];
        }else if(portal.pattern == (PortalPattern) ( LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[12];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[11];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown)){
            tile.sprite = lineTextures[10];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.RightDown & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[9];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[8];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.RightDown)){
            tile.sprite = lineTextures[7];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.LeftDown & LineInPortalPattern.Down)){
            tile.sprite = lineTextures[6];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.LeftDown & LineInPortalPattern.RightDown)){
            tile.sprite = lineTextures[5];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center & LineInPortalPattern.LeftDown)){
            tile.sprite = lineTextures[4];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Down)){
            tile.sprite = lineTextures[3];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.RightDown)){
            tile.sprite = lineTextures[2];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.Center)){
            tile.sprite = lineTextures[1];
        }else if(portal.pattern == (PortalPattern) (LineInPortalPattern.LeftDown)){
            tile.sprite = lineTextures[0];
        }else{
            tile.sprite = lineTextures[15];}
        tileMap.SetTile(new Vector3Int(i, j, 0), tile);
    }
}
