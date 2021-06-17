using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSection
{
    public int width { get; }
    public int height { get; }

    public MapTile[] mapArray { get; set; }
    public MapTile[] backgroundArray { get; set; }
    public MapTile[] decorationsArray { get; set; }
    public MapTile[] trapsArray { get; set; }

    public Vector2Int basePosition { get; set; }
    public Vector2Int entrance {get; set; }
    public Vector2Int exit {get; set; }

    public int xLeftBorder { get; set; }
    public int xRightBorder { get; set; }
    public int yTopBorder { get; set; }
    public int yBottomBorder { get; set; }

    public MapSection(int h, int w, Vector2Int basePosition)
    {
        this.basePosition = basePosition;
        width = w;
        height = h;

        //Declare and initialize sections
        trapsArray = new MapTile[width * height];
        mapArray = new MapTile[width * height];
        backgroundArray = new MapTile[width * height];
        decorationsArray = new MapTile[width * height];
        for (int i = 0; i < width * height; i++)
        {
            trapsArray[i] = new MapTile();
            mapArray[i] = new MapTile();
            backgroundArray[i] = new MapTile();
            decorationsArray[i] = new MapTile();
        }
    }

    public int SetEnemy(int x, int y, GameObject enemy, MapTile[] arr)
    {
        if (-1 < y * width + x && y * width + x < width * height)
        {
            arr[y * width + x].enemy = enemy;
            return 0;
        }
        else
        {
            Debug.LogWarning("'Set enemy' is out of bounds at (" + x + ", " + y + ")");
            return -1;
        }
    }

    public int SetTile(int x, int y, int tileValue, MapTile[] arr)
    {
        if (-1 < y * width + x && y * width + x < width * height)
        {
            arr[y * width + x].tileNum = tileValue;
            return 0;
        }
        else
        {
            Debug.LogWarning("'Set tile' is out of bounds at (" + x + ", " + y + ")");
            return -1;
        }
    }

    public int SetTile(int x, int y, int tileValue, int tilePallete, MapTile[] arr)
    {
        if (-1 < y * width + x && y * width + x < width * height)
        {
            arr[y * width + x].tileNum = tileValue;
            arr[y * width + x].palette = tilePallete;
            return 0;
        }
        else
        {
            Debug.LogWarning("'Set tile' is out of bounds at (" + x + ", " + y + ")");
            return -1;
        }
    }

    public MapTile GetTile(int x, int y, MapTile[] arr)
    {
        if (-1 < y * width + x && y * width + x < width * height)
            return arr[y * width + x];
        else
        {
            Debug.LogWarning("'Get tile' out of bounds at (" + x + ", " + y + ")");
            return null;
        }
        
    }

    public int AddObject(int x, int y, MapSection map, MapTile[] layer, TileObjects obj)
    {
        if (obj.width + x > width - 3 || obj.height + y > height - 3)
        {
            //Debug.LogWarning("Object " + obj.name + " too big for specified location (" + x + ", " + y + ")");
            return -1;
        }

        for (int xi = 0; xi < obj.width; xi++)
        {
            for (int yi = 0; yi < obj.height; yi++)
            {
                map.SetTile(xi + x, yi + y, obj.tileNums[yi * obj.width + xi], obj.palette, layer);
                if (obj.chisel.Length != 0 && obj.chisel[yi * obj.width + xi])
                {
                    map.SetTile(xi + x, yi + y, -1, backgroundArray);
                }
            }
        }
        return 0;
    }

}

public class MapTile
{
    public int tileNum;
    public int palette;
    public GameObject enemy;

    public MapTile()
    {
        this.tileNum = 0;
        this.palette = 1;
        this.enemy = null;
    }

}