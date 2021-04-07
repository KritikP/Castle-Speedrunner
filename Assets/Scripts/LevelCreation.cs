using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelCreation : MonoBehaviour
{
    private Tilemap groundTilemap;
    private Tilemap backgroundTilemap;
    private Tilemap backgroundDecorationTilemap;

    private LinkedList<MapSection> rooms;
    [SerializeField] private Player_Data player;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int minPlatformLength = 2;
    [SerializeField] private int maxPlatformLength = 5;
    [SerializeField] private Vector2Int startingPosition;
    [SerializeField] private TileObjectsList foregroundTiles;
    private int currentRoom;

    private Vector2Int newMapBase;

    private int getWidth()
    {
        return width;
    }

    private int getHeight()
    {
        return height;
    }

    private static bool CoinFlip()
    {
        var rand = new System.Random();
        return rand.Next(2) == 1;
    }

    //Room length must be at least 9 for a spike room.
    private void SpikeRoom(MapSection map)
    {
        ArrayList trajectories = new ArrayList();
        int width = map.width;
        int height = map.height;
        map.entrance = new Vector2Int(map.basePosition.x + 2, map.basePosition.y + 2);
        map.exit = new Vector2Int(map.basePosition.x + width - 1, map.basePosition.y + 2);
        int tileNum = 0;
        int[] spikeRoomLeft = { 345, 384, 408, 438, 451, 482, 502, 534 };
        int[] spikeRoomRight = { 347, 395, 414, 440, 453, 484, 519, 551 };
        int totalSpikes = width - 6;
        bool isChest = CoinFlip();

        //Top tiles
        //Top left corner
        map.SetTile(0, height - 1, 67);
        map.SetTile(1, height - 1, 10);
        map.SetTile(0, height - 2, 12);
        map.SetTile(1, height - 2, 246);

        tileNum = 11;
        for (int c = 2; c < width - 2; c++)
        {
            map.SetTile(c, height - 1, tileNum);
            map.SetTile(c, height - 2, tileNum + 59);
            tileNum++;
            if (tileNum > 20)
                tileNum = 10;
        }

        //Top right corner
        map.SetTile(width - 1, height - 1, 82);
        map.SetTile(width - 2, height - 1, 21);
        map.SetTile(width - 1, height - 2, 20);
        map.SetTile(width - 2, height - 2, 249);

        tileNum = 0;
        for (int r = height - 3; r > 1; r--)
        {
            map.SetTile(0, r, spikeRoomLeft[tileNum]);
            map.SetTile(1, r, spikeRoomLeft[tileNum] + 1);
            map.SetTile(width - 2, r, spikeRoomRight[tileNum]);
            map.SetTile(width - 1, r, spikeRoomRight[tileNum] + 1);
            tileNum++;
            if (tileNum > 7)
                tileNum = 0;
        }

        tileNum = 579;
        for (int c = 2; c < width - 2; c++)
        {
            map.SetTile(c, 1, tileNum);
            map.SetTile(c, 0, tileNum + 33);
            tileNum++;
            if (tileNum > 592)
                tileNum = 579;
        }

        //Bottom Left Corner
        map.SetTile(0, 1, 576);
        map.SetTile(0, 0, 609);
        map.SetTile(1, 1, 577);
        map.SetTile(1, 0, 610);

        //Bottom Right Corner
        map.SetTile(width - 2, 1, 594);
        map.SetTile(width - 1, 1, 595);
        map.SetTile(width - 2, 0, 627);
        map.SetTile(width - 1, 0, 628);
    }

    private void EndlessRunnerSection(MapSection map)
    {
        //Debug.Log("Creating room " + currentRoom);
        int platformTile = 52;

        List<Trajectory> trajectories = new List<Trajectory>();
        int width = map.width;
        int height = map.height;
        //map.entrance = new Vector2Int(map.basePosition.x, map.basePosition.y);
        //map.exit = new Vector2Int(map.basePosition.x + width - 1, map.basePosition.y + 2);

        //Generating platforms
        map.entrance = new Vector2Int(map.basePosition.x + 2, map.basePosition.y + 2);
        map.exit = new Vector2Int(map.basePosition.x + width - 1, map.basePosition.y + 2);
        Vector2Int pathPos = new Vector2Int(map.entrance.x, map.entrance.y);           //Start of generation

        int numPoints = 0;
        Vector2 point = Vector2.zero;
        int col = 0;
        int row = 0;

        var rand = new System.Random();
        int selectedPoint = 0;
        int skipPointsFrac = 3;
        int platformLength = maxPlatformLength - minPlatformLength;
        int platformCount = 0;
        List<int> validPoints = new List<int>();

        map.yTopBorder = map.basePosition.y + height - 1;
        map.yBottomBorder = map.basePosition.y + 2;
        map.xLeftBorder = map.basePosition.x + 2;
        map.xRightBorder = map.basePosition.x + width - 2;

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
                map.SetTile(col + i, row, platformTile);
                //Debug.Log("Set tile at " + (col + i) + ", " + row + " to " + platformTile);
                platformTile++;
                if (platformTile > 58)
                    platformTile = 53;
            }

            if (platformLength != 1)
                map.SetTile(col + platformLength - 1, row, 59);
            else if (platformLength == 1)
                map.SetTile(col + platformLength - 1, row, 52);

            pathPos.x = col + platformLength - 1 + map.basePosition.x;
            pathPos.y = Mathf.FloorToInt(point.y - 1);
            platformCount++;
            validPoints.Clear();
        }
        //Debug.Log("Runner room complete");

    }

    private void DecorateBackground(MapSection map){
        
    }

    private static void RenderMap(MapSection map, Tilemap tilemap)
    {
        int width = map.width;
        int height = map.height;
        Tile tile;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(map.mapArray[y * width + x] != 0)
                {
                    tile = Resources.Load<Tile>("Tiles/Castle_Tiles/main_lev_build_" + map.mapArray[y * width + x]);
                    if (tile == null)
                    {
                        //Debug.Log("Tile " + map.mapArray[r, c] + " not found at " + newTilePosition.x + " ," + newTilePosition.y + " ," + newTilePosition.z);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(map.basePosition.x + x, map.basePosition.y + y, 0), tile);
                        //Debug.Log("Tile " + map.GetTile(x, y) + "loaded at " + x + " ," + y + " ,");
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        groundTilemap = GetComponent<Tilemap>();
        rooms = new LinkedList<MapSection>();
        rooms.AddLast(new MapSection(height, width, startingPosition));
        currentRoom = 0;

        SpikeRoom(rooms.Last.Value);
        EndlessRunnerSection(rooms.Last.Value);
        RenderMap(rooms.Last.Value, groundTilemap);

        if (width <= 6)
        {
            Debug.Log("Width is too small; setting it to 20.");
            width = 30;
        }
        if(height <= 15)
        {
            Debug.Log("Height is too small; setting it to 20.");
            height = 20;
        }

        //Debug.Log("Map length is: " + rooms.Last.Value.width);
        //Debug.Log("Map height is: " + rooms.Last.Value.height);

    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.position.x > rooms.Last.Value.basePosition.x + 0.5f * rooms.Last.Value.width)
        {
            currentRoom++;
            newMapBase = new Vector2Int(rooms.Last.Value.basePosition.x + rooms.Last.Value.width, rooms.Last.Value.basePosition.y);
            Debug.Log("new map with base position:" + newMapBase.x + ", " + newMapBase.y);
            rooms.AddLast(new MapSection(height, width, newMapBase));
            EndlessRunnerSection(rooms.Last.Value);
            RenderMap(rooms.Last.Value, groundTilemap);
        }
    }
}