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

    public Vector2Int basePosition { get; set; }
    public Vector2Int entrance {get; set; }
    public Vector2Int exit {get; set; }

    public int xLeftBorder { get; set; }
    public int xRightBorder { get; set; }
    public int yTopBorder { get; set; }
    public int yBottomBorder { get; set; }

    public char ent;
    public char ext;
    
    public MapSection(int h, int w, Vector2Int basePosition)
    {
        this.basePosition = basePosition;
        width = w;
        height = h;

        //Declare and initialize sections
        mapArray = new MapTile[width * height];
        backgroundArray = new MapTile[width * height];
        decorationsArray = new MapTile[width * height];
        for (int i = 0; i < width * height; i++)
        {
            mapArray[i] = new MapTile();
            backgroundArray[i] = new MapTile();
            decorationsArray[i] = new MapTile();
        }

        ent = 'w';
        ext = 'e';
    }

    public void SetTile(int x, int y, int tileValue, MapTile[] arr)
    {
        arr[y * width + x].tileNum = tileValue;
    }

    public void SetTile(int x, int y, int tileValue, int tilePallete, MapTile[] arr)
    {
        arr[y * width + x].tileNum = tileValue;
        arr[y * width + x].palette = tilePallete;
    }

    public int GetTile(int x, int y, MapTile[] arr)
    {
        return arr[y * width + x].tileNum;
    }

    public void AddObject(int x, int y, MapSection map, MapTile[] layer, TileObjects obj)
    {
        if (obj.width + x > width - 3 || obj.height + y > height - 3)
        {
            Debug.Log("Object too big for specified location");
            return;
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
    }

}

public class MapTile
{
    public int tileNum;
    public int palette;

    public MapTile()
    {
        this.tileNum = 0;
        this.palette = 1;
    }

}