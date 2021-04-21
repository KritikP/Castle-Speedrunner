using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelCreation : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap backgroundDecorationTilemap;

    private LinkedList<MapSection> maps;

    [SerializeField] private Player_Data player;
    [SerializeField] private TileObjectsList tileObjects;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int minPlatformLength = 2;
    [SerializeField] private int maxPlatformLength = 5;
    [SerializeField] private Vector2Int startingPosition;

    private int currentRoom;
    private Vector2Int newMapBase;
    private System.Random rand = new System.Random();

    private int[] background1a = {357, 331, 300, 262, 221, 176, 135};
    private int[] background1b = {230, 185, 144};
    private int[] background2 = {956, 931, 903, 881, 844, 786, 745};
    private int[] background3 = {1506, 1481, 1453, 1431, 1394, 1336, 1295};

    private bool CoinFlip()
    {
        return rand.Next(2) == 1;
    }

    private void GenerateBackground(MapSection map)
    {
        for(int i = 0; i < map.width * map.height; i++)
        {
            map.backgroundArray[i].tileNum = background1a[rand.Next(0, background1a.Length)] + rand.Next(0, 7);
        }
    }

    //Room length must be at least 9 for a spike room.
    private void SpikeRoom(MapSection map)
    {
        int width = map.width;
        int height = map.height;
        int tileNum = 0;
        int[] spikeRoomLeft = { 345, 384, 408, 438, 451, 482, 502, 534 };
        int[] spikeRoomRight = { 347, 395, 414, 440, 453, 484, 519, 551 };
        int totalSpikes = width - 6;
        bool isChest = CoinFlip();
        
        map.yTopBorder = map.basePosition.y + height - 1;
        map.yBottomBorder = map.basePosition.y + 2;
        map.xLeftBorder = map.basePosition.x + 1;
        map.xRightBorder = map.basePosition.x + width - 2;

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

        GenerateBackground(map);
        EndlessRunnerSection(map);
        SpikeFloor(map);
    }

    private void SpikeFloor(MapSection map)
    {
        for(int i = map.xLeftBorder - map.basePosition.x + 1; i < map.xRightBorder - map.basePosition.x; i++)
        {
            map.SetTile(i, map.yBottomBorder - map.basePosition.y - 1, 192, 2, map.mapArray);
        }
    }

    private void EndlessRunnerSection(MapSection map)
    {
        //Debug.Log("Creating room " + currentRoom);
        int platformTile = 52;

        List<Trajectory> trajectories = new List<Trajectory>();
        int width = map.width;
        int height = map.height;

        //Generating platforms
        
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

        while (pathPos.x < map.basePosition.x + width && platformCount < 20)       //While the generation hasn't reached the end of the map section
        {
            //Debug.Log("Platform: " + platformCount);
            platformTile = 52;

            trajectories.Add(new Trajectory(pathPos, player.speed, player.jumpSpeed, player.fallMultiplier, true));
            numPoints = trajectories[platformCount].numPoints;

            for (int i = trajectories[platformCount].numPoints / skipPointsFrac; i < trajectories[platformCount].numPoints; i++)
            {
                if (trajectories[platformCount].points[i].x > map.xRightBorder)
                    break;
                else if (trajectories[platformCount].points[i].y > map.yTopBorder - 3 || trajectories[platformCount].points[i].y < map.yBottomBorder + 1)
                    continue;
                else
                {
                    //Debug.Log("Adding point: " + trajectories[platformCount].points[i]);
                    validPoints.Add(i);
                }
            }

            if (validPoints.Count == 0) //Reached end of section
            {
                //Debug.Log("Reached end of section");
                break;
            }
            selectedPoint = rand.Next(numPoints / skipPointsFrac, validPoints.Count + (numPoints / skipPointsFrac));
            point = trajectories[platformCount].points[selectedPoint];

            col = Mathf.FloorToInt(point.x - map.basePosition.x);
            row = Mathf.FloorToInt(point.y - 1) - map.basePosition.y;

            /*
            Debug.Log("Selected Point " + selectedPoint + ": " + trajectories[platformCount].points[selectedPoint]);
            Debug.Log("Platform " + platformCount + ":");
            Debug.Log("x (column) and y (row): (" + col + ", " + row + ")");
            Debug.Log("Rounded Trajectory point x = " + Mathf.FloorToInt(point.x));
            Debug.Log("Rounded Trajectory point y = " + Mathf.FloorToInt(point.y - 1));
            */

            if ((width - 2) - col < maxPlatformLength)
                platformLength = (width - 2) - col;
            else
                platformLength = rand.Next(minPlatformLength, maxPlatformLength + 1);

            //Debug.Log("Number of platforms generating: " + platformLength);
            for (int i = 0; i < platformLength - 1; i++)
            {
                map.SetTile(col + i, row, platformTile, map.mapArray);
                //Debug.Log("Set tile at " + (col + i) + ", " + row + " to " + platformTile);
                platformTile++;
                if (platformTile > 58)
                    platformTile = 53;
            }

            if (platformLength != 1)
                map.SetTile(col + platformLength - 1, row, 59, map.mapArray);
            else if (platformLength == 1)
                map.SetTile(col + platformLength - 1, row, 52, map.mapArray);

            pathPos.x = col + platformLength - 1 + map.basePosition.x;
            pathPos.y = Mathf.FloorToInt(point.y - 1);
            platformCount++;
            validPoints.Clear();
        }
        Debug.Log("Runner room complete, final path pos at: " + pathPos);
        map.exit = new Vector2Int(pathPos.x, pathPos.y + 1);

    }

    private void DecorateBackground(MapSection map)
    {
        //Start with ground objects
        Vector2Int pos = new Vector2Int(map.xLeftBorder + 2 - map.basePosition.x, map.yBottomBorder - map.basePosition.y);
        TileObjects temp;

        while (pos.x < width)
        {
            temp = tileObjects.GetRandomGroundedTileObject();
            map.AddObject(pos.x, pos.y, map, map.decorationsArray, temp);
            pos.x += temp.width + rand.Next(8, 14);
        }
        
    }

    private void RenderMap(MapSection map)
    {
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
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Create list of map sections
        maps = new LinkedList<MapSection>();

        //Add first section to map
        maps.AddLast(new MapSection(height, width, startingPosition));

        maps.Last.Value.entrance = new Vector2Int(maps.Last.Value.basePosition.x + 2, maps.Last.Value.basePosition.y + (int)(0.3f * maps.Last.Value.height));
        currentRoom = 0;

        //Perform first set of generations
        SpikeRoom(maps.Last.Value);
        RenderMap(maps.Last.Value);

        if (width <= 6)
        {
            Debug.Log("Width is too small; setting it to 20.");
            width = 30;
        }
        if (height <= 15)
        {
            Debug.Log("Height is too small; setting it to 20.");
            height = 20;
        }

        //Debug.Log("Map length is: " + maps.Last.Value.width);
        //Debug.Log("Map height is: " + maps.Last.Value.height);

    }

    // Update is called once per frame
    void Update()
    {
        if (player.playerPosition.position.x > maps.Last.Value.basePosition.x + 0.5f * maps.Last.Value.width)
        {
            currentRoom++;
            newMapBase = new Vector2Int(maps.Last.Value.basePosition.x + maps.Last.Value.width, maps.Last.Value.basePosition.y);
            Debug.Log("New map section with base position: " + newMapBase);
            maps.AddLast(new MapSection(height, width, newMapBase));
            SpikeRoom(maps.Last.Value);
            RenderMap(maps.Last.Value);
        }
    }
}