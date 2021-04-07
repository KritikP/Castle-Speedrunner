﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSection
{
    public int width { get; }
    public int height { get; }
    public int[] mapArray { get; set; }
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
        mapArray = new int[width * height];
        ent = 'w';
        ext = 'e';
    }

    public void SetTile(int x, int y, int tileValue)
    {
        mapArray[y * width + x] = tileValue;
    }

    public int GetTile(int x, int y)
    {
        return mapArray[y * width + x];
    }

}