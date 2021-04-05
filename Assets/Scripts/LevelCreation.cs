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
    [SerializeField] private Player_State player;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int minPlatformLength = 2;
    [SerializeField] private int maxPlatformLength = 5;
    [SerializeField] private Vector2Int startingPosition;
    //[SerializeField] private int seed;
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
        map.mapArray[0, 0] = 67;
        map.mapArray[0, 1] = 10;
        map.mapArray[1, 0] = 12;
        map.mapArray[1, 1] = 246;

        tileNum = 11;
        for (int c = 2; c < width - 2; c++)
        {
            map.mapArray[0, c] = tileNum;
            map.mapArray[1, c] = tileNum + 59;
            tileNum++;
            if (tileNum > 20)
                tileNum = 10;
        }

        //Top right corner
        map.mapArray[0, width - 1] = 82;
        map.mapArray[0, width - 2] = 21;
        map.mapArray[1, width - 1] = 20;
        map.mapArray[1, width - 2] = 249;

        tileNum = 0;
        for (int r = 2; r < height - 2; r++)
        {
            map.mapArray[r, 0] = spikeRoomLeft[tileNum];
            map.mapArray[r, 1] = spikeRoomLeft[tileNum] + 1;
            map.mapArray[r, width - 2] = spikeRoomRight[tileNum];
            map.mapArray[r, width - 1] = spikeRoomRight[tileNum] + 1;
            tileNum++;
            if (tileNum > 7)
                tileNum = 0;
        }

        tileNum = 579;
        for (int c = 2; c < width - 2; c++)
        {
            map.mapArray[height - 2, c] = tileNum;
            map.mapArray[height - 1, c] = tileNum + 33;
            tileNum++;
            if (tileNum > 592)
                tileNum = 579;
        }

        //Bottom Left Corner
        map.mapArray[height - 2, 0] = 576;
        map.mapArray[height - 1, 0] = 609;
        map.mapArray[height - 2, 1] = 577;
        map.mapArray[height - 1, 1] = 610;

        //Bottom Right Corner
        map.mapArray[height - 2, width - 2] = 594;
        map.mapArray[height - 2, width - 1] = 595;
        map.mapArray[height - 1, width - 2] = 627;
        map.mapArray[height - 1, width - 1] = 628;
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
                Debug.Log("Reached end of section");
                break;
            }
            selectedPoint = rand.Next(numPoints / skipPointsFrac, validPoints.Count + (numPoints / skipPointsFrac));
            point = trajectories[platformCount].points[selectedPoint];

            col = Mathf.FloorToInt(point.x - map.basePosition.x);
            row = (map.height - 1) - (Mathf.FloorToInt(point.y - 1) - map.basePosition.y);

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
                map.mapArray[row, col + i] = platformTile;
                platformTile++;
                if (platformTile > 58)
                    platformTile = 53;
            }

            if (platformLength != 1)
                map.mapArray[row, col + platformLength - 1] = 59;
            else if (platformLength == 1)
                map.mapArray[row, col + platformLength - 1] = 52;

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
        Vector2Int newTilePosition = map.basePosition;
        for (int r = height - 1; r >= 0; r--)
        {
            for (int c = 0; c < width; c++)
            {
                if(map.mapArray[r, c] != 0)
                {
                    tile = Resources.Load<Tile>("Tiles/Castle_Tiles/main_lev_build_" + map.mapArray[r, c]);
                    if (tile == null)
                    {
                        //Debug.Log("Tile " + map.mapArray[r, c] + " not found at " + newTilePosition.x + " ," + newTilePosition.y + " ," + newTilePosition.z);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(newTilePosition.x, newTilePosition.y, 0), tile);
                        //Debug.Log("Tile loaded at " + newTilePosition.x + " ," + newTilePosition.y + " ,");
                    }
                }
                newTilePosition.x = newTilePosition.x + 1;
            }
            newTilePosition.x = map.basePosition.x;
            newTilePosition.y = newTilePosition.y + 1;
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
