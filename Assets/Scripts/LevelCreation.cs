using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelCreation : MonoBehaviour
{
    [SerializeField] private Tilemap trapsTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap backgroundDecorationTilemap;
    [SerializeField] private Map_Data mapData;
    [SerializeField] private TrapSpawner trapSpawner;

    private List<MapSection> maps;

    [SerializeField] private Player_Data player;
    [SerializeField] private TileObjectsList tileObjects;
    private GameObject[] enemiesList;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int minPlatformLength = 2;
    [SerializeField] private int maxPlatformLength = 5;
    [SerializeField] private Vector2Int startingPosition = Vector2Int.zero;
    
    private int entrancePlatformLength = 4;
    private int currentRoom;
    private System.Random rand = new System.Random();

    private int[] background1a = {357, 331, 300, 262, 221, 176, 135};
    private int[] background1b = {230, 185, 144};
    private int[] background2 = {956, 931, 903, 881, 844, 786, 745};
    private int[] background3 = {1506, 1481, 1453, 1431, 1394, 1336, 1295};
    private int[] spikeRoomLeft = { 345, 384, 408, 438, 451, 482, 502, 534 };
    private int[] spikeRoomRight = { 347, 395, 414, 440, 453, 484, 519, 551 };

    private GameObject glowTile1;
    private GameObject glowTile2;

    [SerializeField] private int                  maxEnemiesPerRoom = 3;
    [SerializeField, Range(0, 1)] private float   enemySpawnRatePerPlatform = 0.2f;
    [SerializeField] private int                  maxCoinSetsPerRoom = 2;
    [SerializeField, Range(0, 1)] private float   coinSpawnRatePerPlatform = 0.2f;
    [SerializeField] private int                  maxTrapsPerRoom = 2;
    [SerializeField, Range(0, 1)] private float   powerUpSpawnChance = 0.1f;
    [SerializeField] private int                  maxPowerUpsPerRoom = 1;

    private void GenerateBackground(MapSection map)
    {
        for(int i = 0; i < map.width * map.height; i++)
        {
            map.backgroundArray[i].tileNum = background1a[rand.Next(0, background1a.Length)] + rand.Next(0, 7);
        }
    }

    private void CreateRoomBase(MapSection map)
    {
        int tileNum = 0;
        
        map.yTopBorder = map.height - 2;
        map.yBottomBorder = 1;
        map.xLeftBorder = 1;
        map.xRightBorder = map.width - 2;

        //Top tiles
        //Top left corner
        map.SetTile(0, height - 1, 67, map.mapArray);
        map.SetTile(1, height - 1, 10, map.mapArray);
        map.SetTile(0, height - 2, 12, map.mapArray);
        map.SetTile(1, height - 2, 246, map.mapArray);

        tileNum = 11;
        for (int c = 2; c < width - 2; c++)
        {
            map.SetTile(c, height - 1, tileNum, map.mapArray);
            map.SetTile(c, height - 2, tileNum + 59, map.mapArray);
            tileNum++;
            if (tileNum > 20)
                tileNum = 10;
        }

        //Top right corner
        map.SetTile(width - 1, height - 1, 82, map.mapArray);
        map.SetTile(width - 2, height - 1, 21, map.mapArray);
        map.SetTile(width - 1, height - 2, 20, map.mapArray);
        map.SetTile(width - 2, height - 2, 249, map.mapArray);

        tileNum = 0;
        for (int r = height - 3; r > 1; r--)
        {
            map.SetTile(0, r, spikeRoomLeft[tileNum], map.mapArray);
            map.SetTile(1, r, spikeRoomLeft[tileNum] + 1, map.mapArray);
            map.SetTile(width - 2, r, spikeRoomRight[tileNum], map.mapArray);
            map.SetTile(width - 1, r, spikeRoomRight[tileNum] + 1, map.mapArray);
            tileNum++;
            if (tileNum > 7)
                tileNum = 0;
        }

        tileNum = 579;
        for (int c = 2; c < width - 2; c++)
        {
            map.SetTile(c, 1, tileNum, map.mapArray);
            map.SetTile(c, 0, tileNum + 33, map.mapArray);
            tileNum++;
            if (tileNum > 592)
                tileNum = 579;
        }

        //Bottom Left Corner
        map.SetTile(0, 1, 576, map.mapArray);
        map.SetTile(0, 0, 609, map.mapArray);
        map.SetTile(1, 1, 577, map.mapArray);
        map.SetTile(1, 0, 610, map.mapArray);

        //Bottom Right Corner
        map.SetTile(width - 2, 1, 594, map.mapArray);
        map.SetTile(width - 1, 1, 595, map.mapArray);
        map.SetTile(width - 2, 0, 627, map.mapArray);
        map.SetTile(width - 1, 0, 628, map.mapArray);

        //Chisel the entrance area
        
        if (currentRoom != 0)
        {
            for(int x = 1; x >= 0; x--)
            {
                for(int y = map.entrance.y; y < map.entrance.y + 6; y++)
                {
                    map.SetTile(x, y, -1, map.mapArray);
                    map.SetTile(x, y, -1, map.mapArray);
                }
            }
            map.SetTile(1, map.entrance.y, 593, map.mapArray);
            map.SetTile(1, map.entrance.y + 5, 290, map.mapArray);
            map.SetTile(0, map.entrance.y, 1092, map.mapArray);
            map.SetTile(0, map.entrance.y + 5, 1092, map.mapArray);

            Instantiate(glowTile1, new Vector3(map.basePosition.x + 0.5f, map.entrance.y + 0.5f, 0), Quaternion.identity);
            Instantiate(glowTile1, new Vector3(map.basePosition.x + 0.5f, map.entrance.y + 0.5f + 5, 0), Quaternion.identity);
        }

        GenerateBackground(map);
    }

    private void SpikeFloor(MapSection map)
    {
        for(int i = map.xLeftBorder + 1; i < map.xRightBorder; i++)
        {
            map.SetTile(i, map.yBottomBorder, -1, map.mapArray);
            map.SetTile(i, map.yBottomBorder, 192, 2, map.trapsArray);
        }
    }

    private void CreateTrapFloor(MapSection map)
    {
        int trapSectionWidth = (map.width - 10) / maxTrapsPerRoom;
        if (trapSectionWidth < 1)
            return;
        
        for(int i = 0; i < maxTrapsPerRoom; i++)
        {
            trapSpawner.SpawnShurikenTrap(new Vector3(map.basePosition.x + 10 + i * trapSectionWidth + 0.5f, map.yBottomBorder, 0));
            map.SetTile(10 + i * trapSectionWidth, map.yBottomBorder, 505, map.mapArray);
            map.SetTile(10 + i * trapSectionWidth, map.yBottomBorder - 1, 537, map.mapArray);

        }
    }

    private void CreatePlatforms(MapSection map)
    {
        int platformTile = 56;
        int enemyCount = 0;
        int coinSetCount = 0;
        int powerUpsCount = 0;

        List<Trajectory> trajectories = new List<Trajectory>();
        width = map.width;
        height = map.height;

        Vector2Int pathPos = new Vector2Int(map.entrance.x, map.entrance.y);           //Start of generation
        
        int numPoints = 0;
        Vector2 point = Vector2.zero;
        int col = 0;
        int row = 0;

        int selectedPoint = 0;
        int skipPointsFrac = 3;
        int platformLength = maxPlatformLength - minPlatformLength;
        int platformCount = 0;
        List<int> validPoints = new List<int>();

        for (int i = 0; i < entrancePlatformLength; i++)
        {
            map.SetTile(pathPos.x, pathPos.y, platformTile + i, map.mapArray);
            pathPos.x = pathPos.x + 1;
        }
        
        pathPos.x = pathPos.x - 1;

        platformTile = 52;
        while (pathPos.x < map.width)       //While the generation hasn't reached the end of the map section
        {
            platformTile = 52;

            trajectories.Add(new Trajectory(pathPos, player.speed, player.jumpSpeed, player.fallMultiplier, true));
            numPoints = trajectories[platformCount].numPoints;

            int platformX;
            int platformY;
            //Debug.Log("Path pos" + pathPos);
            for (int i = numPoints / skipPointsFrac; i < numPoints; i++)
            {
                platformX = Mathf.FloorToInt(trajectories[platformCount].points[i].x);
                platformY = Mathf.FloorToInt(trajectories[platformCount].points[i].y - 1f);
                if (platformX > map.xRightBorder)
                {
                    break;
                }
                else if (platformY > map.yTopBorder - 7 || platformY < map.yBottomBorder + 3)
                {

                }
                else
                {
                    validPoints.Add(i);
                }
            }

            if (validPoints.Count == 0) //Reached end of section
            {
                //Debug.Log("Reached end of section");
                break;
            }
            selectedPoint = validPoints[rand.Next(0, validPoints.Count)];
            point = trajectories[platformCount].points[selectedPoint];

            //Debug.Log("Selected point: " + point);
            col = Mathf.FloorToInt(point.x);
            row = Mathf.FloorToInt(point.y - 1);

            if ((width - 2) - col < maxPlatformLength)
                platformLength = (width - 2) - col;
            else
                platformLength = rand.Next(minPlatformLength, maxPlatformLength + 1);

            //Debug.Log("Number of blocks generating: " + platformLength);
            for (int i = 0; i < platformLength - 1; i++)
            {
                map.SetTile(col + i, row, platformTile, map.mapArray);
                platformTile++;
                if (platformTile > 58)
                    platformTile = 53;
            }

            if (platformLength != 1)
                map.SetTile(col + platformLength - 1, row, 59, map.mapArray);
            else if (platformLength == 1)
                map.SetTile(col + platformLength - 1, row, 52, map.mapArray);

            //Set enemies
            if(enemyCount < maxEnemiesPerRoom && platformLength > 2)
            {
                if (rand.Next(0, 100) < enemySpawnRatePerPlatform * 100)
                {
                    //Spawn
                    map.SetEnemy(col + platformLength - 1, row + 1, enemiesList[rand.Next(0, enemiesList.Length)], map.mapArray);
                    enemyCount++;
                }
            }

            if(powerUpsCount < maxPowerUpsPerRoom)
            {
                if (rand.Next(0, 100) < powerUpSpawnChance * 100)
                {
                    TileObjects obj = tileObjects.GetTileObject("arch_small1");
                    if(map.AddObject(col + platformLength / 2, row + 2, map, map.decorationsArray, obj) == 0)
                    {
                        Instantiate(obj.glowObject, new Vector3(map.basePosition.x + col + platformLength / 2 + obj.glowPosition.x, map.basePosition.y + row + 2 + obj.glowPosition.y), Quaternion.identity);
                        powerUpsCount++;
                    }
                    
                }
            }

            //Set coins
            if (coinSetCount < maxCoinSetsPerRoom)
            {
                if (rand.Next(0, 100) < coinSpawnRatePerPlatform * 100)
                {
                    //Create coins
                    for(int i = 0; validPoints[i] != selectedPoint; i++)
                    {
                        Vector3 pos = new Vector3(map.basePosition.x + trajectories[platformCount].points[validPoints[i]].x, map.basePosition.y + trajectories[platformCount].points[validPoints[i]].y + 1, 0);
                        Instantiate(Resources.Load("Prefabs/Items/Coin"), pos, Quaternion.identity);
                    }
                    coinSetCount++;
                }
            }

            pathPos.x = col + (platformLength - 1);
            pathPos.y = Mathf.FloorToInt(point.y - 1);
            platformCount++;
            validPoints.Clear();
        }
        map.exit = new Vector2Int(map.width - 3, pathPos.y);

        //Chisel and set exit area
        map.SetTile(width - 2, pathPos.y, 578, map.mapArray);
        map.SetTile(width - 2, pathPos.y + 5, 291, map.mapArray);
        map.SetTile(width - 1, pathPos.y, 1089, map.mapArray);
        map.SetTile(width - 1, pathPos.y + 5, 1089, map.mapArray);
        for(int i = 1; i < 5; i++)
        {
            map.SetTile(map.width - 2, pathPos.y + i, -1, map.mapArray);
            map.SetTile(map.width - 1, pathPos.y + i, -1, map.mapArray);
        }
        Instantiate(glowTile2, new Vector3(map.basePosition.x + map.width - 1 + 0.5f, map.exit.y + 0.5f, 0), Quaternion.identity);
        Instantiate(glowTile2, new Vector3(map.basePosition.x + map.width - 1 + 0.5f, map.exit.y + 0.5f + 5, 0), Quaternion.identity);
        trajectories.Clear();
    }

    private void TransitionArea(MapSection map)
    {
        GenerateBackground(map);
        int glowTile = 1090;
        int brickTiles = 612;
        int blackTile = 288;

        for(int i = 0; i < map.width; i++)
        {
            map.SetTile(map.entrance.x + i, map.entrance.y, glowTile, map.mapArray);
            map.SetTile(map.entrance.x + i, map.entrance.y + 5, glowTile, map.mapArray);

            map.SetTile(map.entrance.x + i, map.entrance.y - 1, brickTiles, map.backgroundArray);
            map.SetTile(map.entrance.x + i, map.entrance.y + 6, brickTiles, map.backgroundArray);
            brickTiles++;
            if (brickTiles > 625)
                brickTiles = 612;

            for(int c = map.entrance.y + 7; c < map.height; c++)
            {
                map.SetTile(map.entrance.x + i, c, blackTile, map.backgroundArray);
            }
            for (int c = map.entrance.y - 2; c >= 0; c--)
            {
                map.SetTile(map.entrance.x + i, c, blackTile, map.backgroundArray);
            }
        }

        map.exit = new Vector2Int(map.width - 1, map.entrance.y);
    }

    private void DecorateGround(MapSection map)
    {
        TileObjects temp;

        //Start with ground objects
        Vector2Int pos = new Vector2Int(map.xLeftBorder + 2, map.yBottomBorder + 1);
        while (pos.x < map.width - 3)
        {
            temp = tileObjects.GetRandomGroundedTileObject();
            if (map.AddObject(pos.x, pos.y, map, map.decorationsArray, temp) != 0)
                break;
            pos.x += temp.width + rand.Next(8, 14);
        }
        
    }

    private void DecorateWalls(MapSection map)
    {
        TileObjects temp;

        //Wall Objects
        Vector2Int pos = new Vector2Int(map.xLeftBorder + 2, map.yBottomBorder + 10);
        while (pos.x < map.width - 3)
        {
            temp = tileObjects.GetRandomTileObject();
            if (map.AddObject(pos.x, pos.y, map, map.decorationsArray, temp) != 0)
                break;
            pos.x += temp.width + rand.Next(5, 9);
        }
    }

    private void RenderMap(MapSection map)
    {
        RenderMapHelper(map, map.trapsArray, trapsTilemap);
        RenderMapHelper(map, map.mapArray, groundTilemap);
        RenderMapHelper(map, map.backgroundArray, backgroundTilemap);
        RenderMapHelper(map, map.decorationsArray, backgroundDecorationTilemap);
    }

    private void RenderMapHelper(MapSection map, MapTile[] arr, Tilemap tilemap)
    {
        int width = map.width;
        int height = map.height;
        Tile tile;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (arr[y * width + x].tileNum > 0)
                {
                    if (arr[y * width + x].palette == 1)
                    {
                        tile = Resources.Load<Tile>("Tiles/Castle_Tiles/main_lev_build_" + arr[y * width + x].tileNum);
                    }
                    else
                    {
                        tile = Resources.Load<Tile>("Tiles/Castle_Tiles/other_and_decorative_" + arr[y * width + x].tileNum);
                    }

                    if (tile == null)
                    {
                        //Debug.Log("Tile " + arr[y * width + x] + " not found at " + x + " ," + y + " ,");
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(map.basePosition.x + x, map.basePosition.y + y, 0), tile);
                        //Debug.Log("Tile " + map.GetTile(x, y) + "loaded at " + x + " ," + y + " ,");
                    }
                }
                else if(arr[y * width + x].tileNum == -1)
                {
                    tilemap.SetTile(new Vector3Int(map.basePosition.x + x, map.basePosition.y + y, 0), null);
                }

                if(arr[y * width + x].enemy != null)
                {
                    Instantiate(arr[y * width + x].enemy, new Vector3(map.basePosition.x + x, map.basePosition.y + y + 1, 0), Quaternion.identity);
                }
            }
        }
    }

    private void AddSpikeRoom()
    {
        Vector2Int newMapBase;
        Vector2Int newMapEntrance;

        if (mapData.mapSections.Count == 0)
        {
            newMapBase = startingPosition;
            newMapEntrance = new Vector2Int(2, (int)(0.3f * height));
        }
        else
        {
            newMapBase = new Vector2Int(mapData.mapSections[currentRoom].basePosition.x + mapData.mapSections[currentRoom].width, mapData.mapSections[currentRoom].basePosition.y);
            newMapEntrance = new Vector2Int(2, mapData.mapSections[currentRoom].exit.y);
            currentRoom++;
        }

        Debug.Log("New map section with base position: " + newMapBase);
        mapData.mapSections.Add(new MapSection(height, width, newMapBase));
        mapData.mapSections[currentRoom].entrance = newMapEntrance;
        CreateRoomBase(mapData.mapSections[currentRoom]);
        CreatePlatforms(mapData.mapSections[currentRoom]);
        SpikeFloor(mapData.mapSections[currentRoom]);
        //DecorateWalls(mapData.mapSections[currentRoom]);
        RenderMap(mapData.mapSections[currentRoom]);
    }

    private void AddTrappedSpikeRoom()
    {
        Vector2Int newMapBase;
        Vector2Int newMapEntrance;

        if (mapData.mapSections.Count == 0)
        {
            newMapBase = startingPosition;
            newMapEntrance = new Vector2Int(2, (int)(0.3f * height));
        }
        else
        {
            newMapBase = new Vector2Int(mapData.mapSections[currentRoom].basePosition.x + mapData.mapSections[currentRoom].width, mapData.mapSections[currentRoom].basePosition.y);
            newMapEntrance = new Vector2Int(2, mapData.mapSections[currentRoom].exit.y);
            currentRoom++;
        }

        Debug.Log("New map section with base position: " + newMapBase);
        mapData.mapSections.Add(new MapSection(height, width, newMapBase));
        mapData.mapSections[currentRoom].entrance = newMapEntrance;
        CreateRoomBase(mapData.mapSections[currentRoom]);
        CreatePlatforms(mapData.mapSections[currentRoom]);
        CreateTrapFloor(mapData.mapSections[currentRoom]);
        SpikeFloor(mapData.mapSections[currentRoom]);
        DecorateWalls(mapData.mapSections[currentRoom]);
        RenderMap(mapData.mapSections[currentRoom]);
        
    }

    private void AddTransitionRoom()
    {
        Vector2Int newMapBase;
        Vector2Int newMapEntrance;

        if (mapData.mapSections.Count == 0)
        {
            newMapBase = startingPosition;
            newMapEntrance = new Vector2Int(2, (int)(0.3f * height));
        }
        else
        {
            newMapBase = new Vector2Int(mapData.mapSections[currentRoom].basePosition.x + mapData.mapSections[currentRoom].width, mapData.mapSections[currentRoom].basePosition.y);
            newMapEntrance = new Vector2Int(0, mapData.mapSections[currentRoom].exit.y);
            currentRoom++;
        }
        
        Debug.Log("Transition map section with base position: " + newMapBase);
        mapData.mapSections.Add(new MapSection(height, 2, newMapBase));
        mapData.mapSections[currentRoom].entrance = newMapEntrance;
        TransitionArea(mapData.mapSections[currentRoom]);
        RenderMap(mapData.mapSections[currentRoom]);
    }

    private void AddStandardRoom()
    {
        Vector2Int newMapBase;
        Vector2Int newMapEntrance;

        if (mapData.mapSections.Count == 0)
        {
            newMapBase = startingPosition;
            newMapEntrance = new Vector2Int(2, (int)(0.3f * height));
        }
        else
        {
            newMapBase = new Vector2Int(mapData.mapSections[currentRoom].basePosition.x + mapData.mapSections[currentRoom].width, mapData.mapSections[currentRoom].basePosition.y);
            newMapEntrance = new Vector2Int(2, mapData.mapSections[currentRoom].exit.y);
            currentRoom++;
        }
 
        Debug.Log("New map section with base position: " + newMapBase);
        mapData.mapSections.Add(new MapSection(height, width, newMapBase));
        mapData.mapSections[currentRoom].exit = new Vector2Int(mapData.mapSections[currentRoom].width - 3, 2);
        mapData.mapSections[currentRoom].entrance = newMapEntrance;
        CreateRoomBase(mapData.mapSections[currentRoom]);
        CreateStandardRoom(mapData.mapSections[currentRoom]);
        CreateTrapFloor(mapData.mapSections[currentRoom]);
        DecorateGround(mapData.mapSections[currentRoom]);
        DecorateWalls(mapData.mapSections[currentRoom]);
        RenderMap(mapData.mapSections[currentRoom]);
    }

    private void CreateStandardRoom(MapSection map)
    {
        //Chisel and set exit area
        map.SetTile(width - 2, 2, 578, map.mapArray);
        map.SetTile(width - 2, 2 + 5, 291, map.mapArray);
        map.SetTile(width - 1, 2, 1089, map.mapArray);
        map.SetTile(width - 1, 2 + 5, 1089, map.mapArray);
        for (int i = 1; i < 5; i++)
        {
            map.SetTile(map.width - 2, 2 + i, -1, map.mapArray);
            map.SetTile(map.width - 1, 2 + i, -1, map.mapArray);
        }
        Instantiate(glowTile2, new Vector3(map.basePosition.x + map.width - 1 + 0.5f, map.exit.y + 0.5f, 0), Quaternion.identity);
        Instantiate(glowTile2, new Vector3(map.basePosition.x + map.width - 1 + 0.5f, map.exit.y + 0.5f + 5, 0), Quaternion.identity);

        //Set enemies
        int enemySpawnGap = (map.width - 4) / maxEnemiesPerRoom;
        for(int i = 0; i < maxEnemiesPerRoom; i++)
        {
            //Spawn
            map.SetEnemy(i * enemySpawnGap + 4, map.yBottomBorder + 1, enemiesList[rand.Next(0, enemiesList.Length)], map.mapArray);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemiesList = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        glowTile1 = (GameObject)Resources.Load("Lighting/Freeform Light 2D_GlowTile1");
        glowTile2 = (GameObject)Resources.Load("Lighting/Freeform Light 2D_GlowTile2");
        
        mapData.mapSections.Clear();

        //Add and generate first section to map
        currentRoom = 0;

        if (width < 20)
        {
            Debug.LogWarning("Map width is too small; setting it to 20.");
            width = 20;
        }
        if (height < 20)
        {
            Debug.LogWarning("Map height is too small; setting it to 20.");
            height = 20;
        }

        AddSpikeRoom();

        for(int i = 0; i < 10; i++)
        {
            AddTransitionRoom();
            if (rand.Next(0, 2) == 1)
            {
                AddStandardRoom();
            }
            else
            {
                AddTrappedSpikeRoom();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}