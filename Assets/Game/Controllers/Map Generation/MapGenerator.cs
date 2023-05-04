using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using NavMeshPlus.Components;
using System.Linq;
using UnityEngine.Rendering.Universal;

public class MapGenerator : MonoBehaviour
{
    [Header("Tilemaps and Tiles")]
    public Tilemap wallTilemap;
    public Tilemap roofTilemap;
    public Tilemap floorTilemap;
    public Tilemap unwalkableDecorationTilemap;
    public Tilemap walkableDecorationTilemap;
    public RuleTile wallTiles;
    public RuleTile roofTiles;
    public RuleTile floorTiles;
    public RuleTile unwalkableDecorationTiles;
    public RuleTile walkableDecorationTiles;
    [Header("Map Generation Settings")]
    public int width;
    public int height;
    public int seed;
    public bool useRandomSeed = true;
    public int minRoomSize;
    public int maxRoomSize;
    public int minRooms;
    public int maxRooms;
    public int minRoomDistance = 2;
    public int maxRoomDistance;
    public int level = 0;
    System.Random pseudoRandom;
    public int agentAmount;
    public int walkMin;
    public int walkMax;
    public float minHallwaysMultiplier = 1;
    public float maxHallwayMultiplier = 3;
    [Header("Decoration Settings")]
    public int decorMin;
    public int decorMax;
    [Header("Prefabs")]
    public GameObject propsContainer;
    public GameObject[] lightPrefabs;
    public GameObject[] enemyPrefabs;
    public int enemyMin;
    public int enemyMax;
    public GameObject[] chestPrefabs;
    public int chestChance;
    public GameObject bonfirePrefab;
    public GameObject exitPrefab;

    [Header("Debug")]
    public bool debugRooms;
    private DebugRoom startRoom;
    public List<DebugRoom> rooms = new List<DebugRoom>();
    public List<Rect> chestRooms = new List<Rect>();
    public Dictionary<Rect, GameObject> chests = new Dictionary<Rect, GameObject>();
    public List<Rect[]> hallways = new List<Rect[]>();
    public bool debugPoints;
    public List<Vector2> pointsToDebug = new List<Vector2>();
    public bool debugLines;
    public List<DebugPoint> linesToDebug = new List<DebugPoint>();
    public bool alwaysGenerate;
    public GameController gameController;

    public void Start()
    {
        if (alwaysGenerate)
        {
            GenerateMap();
            gameController.StartGame();
            gameController.player.enabled = false;
        }
    }

    public bool GenerateMap()
    {
        var watch = new System.Diagnostics.Stopwatch();

        watch.Start();

        if (useRandomSeed)
        {
            seed = UnityEngine.Random.Range(0, 1000000);
        }
        pseudoRandom = new System.Random((seed + level).GetHashCode());
        ClearMap();
        CreateRooms();
        Invoke("CreateNav", 0.1f);

        watch.Stop();
        Debug.Log($"Execution Time: {watch.ElapsedMilliseconds} ms");
        return true;
    }

    void CreateNav()
    {
        GameObject.FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        PlaceEnemies();
    }

    public void ClearMap()
    {
        pointsToDebug.Clear();
        linesToDebug.Clear();
        chestRooms.Clear();
        hallways.Clear();
        rooms.Clear();
        DestroyImmediate(propsContainer);
        propsContainer = new GameObject("Props");
        propsContainer.transform.parent = transform;
        wallTilemap.ClearAllTiles();
        roofTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
        unwalkableDecorationTilemap.ClearAllTiles();
        walkableDecorationTilemap.ClearAllTiles();
        int extra = 5;
        for (int x = -extra; x < (width + extra) * 2; x++)
        {
            for (int y = -extra; y < height + extra; y++)
            {
                wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTiles);
            }
            for (int y = -extra * 2; y < (height + extra) * 2; y++)
            {
                roofTilemap.SetTile(new Vector3Int(x, y, 0), roofTiles);
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTiles);
            }
        }
    }

    public void CreateRooms()
    {
        rooms.Clear();
        int roomCount = pseudoRandom.Next(minRooms, maxRooms);
        for (int i = 0; i < roomCount; i++)
        {
            int roomWidth = pseudoRandom.Next(minRoomSize, maxRoomSize);
            int roomHeight = pseudoRandom.Next(minRoomSize / 2, maxRoomSize / 2);
            int roomX = pseudoRandom.Next(0, (width * 2) - roomWidth);
            int roomY = pseudoRandom.Next(0, height - roomHeight);
            Rect room = new Rect(roomX, roomY, roomWidth, roomHeight);
            bool overlaps = false;
            foreach (Rect otherRoom in rooms.Select(r => r.room).ToList())
            {
                Rect sizedUpRoom = otherRoom;
                sizedUpRoom.xMin -= minRoomDistance;
                sizedUpRoom.xMax += minRoomDistance;
                sizedUpRoom.yMin -= minRoomDistance;
                sizedUpRoom.yMax += minRoomDistance;
                if (room.Overlaps(sizedUpRoom))
                {
                    overlaps = true;
                    break;
                }
            }
            if (!overlaps)
            {
                rooms.Add(new DebugRoom(room, Color.red));
            }
        }
        foreach (Rect room in rooms.Select(r => r.room).ToList())
        {
            //Walls are 2 tiles tall and 1 tile wide
            for (int x = (int)room.xMin; x < room.xMax; x++)
            {
                for (int y = (int)room.yMin; y < room.yMax; y++)
                {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x, y * 2, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 1, 0), null);
                }
            }
            //Remove ceiling that's 2 above rooms upper limit
            for (int x = (int)room.xMin; x < room.xMax; x++)
            {
                roofTilemap.SetTile(new Vector3Int(x, (int)room.yMax * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(x, ((int)room.yMax * 2) + 1, 0), null);
            }
            //Do random walk remove tiles around room
            for (int agent = 0; agent < agentAmount; agent++)
            {
                int walkLength = pseudoRandom.Next(walkMin, walkMax);
                int walkX = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                int walkY = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                for (int i = 0; i < walkLength; i++)
                {
                    int direction = pseudoRandom.Next(0, 4);
                    switch (direction)
                    {
                        case 0:
                            walkX++;
                            break;
                        case 1:
                            walkX--;
                            break;
                        case 2:
                            walkY++;
                            break;
                        case 3:
                            walkY--;
                            break;
                    }
                    if (walkX < 2 || walkX >= (width * 2) - 2 || walkY < 2 || walkY >= height - 2)
                    {
                        continue;
                    }
                    wallTilemap.SetTile(new Vector3Int(walkX, walkY, 0), null);
                    roofTilemap.SetTile(new Vector3Int(walkX, walkY * 2, 0), null);
                    roofTilemap.SetTile(new Vector3Int(walkX, (walkY * 2) + 1, 0), null);
                }
            }
        }
        //Make minimum span tree between rooms
        List<Rect> minimumSpanTree = new List<Rect>();
        List<Rect> MSPRooms = new List<Rect>(this.rooms.Select(r => r.room).ToList());
        minimumSpanTree.Add(MSPRooms[0]);
        MSPRooms.RemoveAt(0);
        while (MSPRooms.Count > 0)
        {
            float closestDistance = float.MaxValue;
            Rect closestRoom = new Rect();
            Rect closestMinimumSpanTreeRoom = new Rect();
            foreach (Rect room in MSPRooms)
            {
                foreach (Rect minimumSpanTreeRoom in minimumSpanTree)
                {
                    float distance = Vector2.Distance(new Vector2(room.x, room.y), new Vector2(minimumSpanTreeRoom.x, minimumSpanTreeRoom.y));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestRoom = room;
                        closestMinimumSpanTreeRoom = minimumSpanTreeRoom;
                    }
                }
            }

            minimumSpanTree.Add(closestRoom);
            hallways.Add(new Rect[] { closestRoom, closestMinimumSpanTreeRoom });
            MSPRooms.Remove(closestRoom);
        }
        int failsafe = 0;
        while (hallways.Count < Mathf.RoundToInt((float)rooms.Count * ((float)pseudoRandom.Next(Mathf.RoundToInt(minHallwaysMultiplier * 10), Mathf.RoundToInt(maxHallwayMultiplier * 10)) / 10f)))
        {
            failsafe++;
            // Debug.Log("Failsafe: " + failsafe + "\nHallways: " + hallways.Count + "\nRooms: " + rooms.Count + "\nRooms / 2: " + rooms.Count / 2 + "\n");
            if (failsafe > rooms.Count * 2)
            {
                break;
            }
            Rect startRoom = rooms.Select(r => r.room).ToList()[pseudoRandom.Next(0, rooms.Count)];
            Rect endRoom = rooms.Select(r => r.room).ToList()[pseudoRandom.Next(0, rooms.Count)];
            if (startRoom == endRoom)
            {
                continue;
            }
            if (hallways.Contains(new Rect[] { startRoom, endRoom }) || hallways.Contains(new Rect[] { endRoom, startRoom }))
            {
                continue;
            }
            hallways.Add(new Rect[] { startRoom, endRoom });
        }
        foreach (Rect[] hallway in hallways)
        {
            Rect startRoom = hallway[0];
            Rect endRoom = hallway[1];
            Vector2Int[] bestCoordinateMatch = BestCoordinateMatch(startRoom, endRoom);
            //Make hallways between rooms using A*
            List<Vector2Int> path = AStar(bestCoordinateMatch[0], bestCoordinateMatch[1]);
            Vector2Int previousTile = path[0];
            foreach (Vector2Int tile in path)
            {
                linesToDebug.Add(new DebugPoint(new Vector2[] { previousTile, tile }, Color.red));
                wallTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x, tile.y * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x, (tile.y * 2) + 1, 0), null);
                wallTilemap.SetTile(new Vector3Int(tile.x + 1, tile.y, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x + 1, tile.y * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x + 1, (tile.y * 2) + 1, 0), null);
                previousTile = tile;
            }
        }

        CleanUp();
        PlaceProps();
        PlaceBonfire();
        PlaceChests();
    }

    private Vector2Int[] BestCoordinateMatch(Rect startRoom, Rect endRoom)
    {
        Vector2Int[] bestCoordinateMatch = new Vector2Int[2];
        float bestDistance = float.MaxValue;
        Vector2Int[] startRoomCoordinates =
        {
            new Vector2Int((int)startRoom.xMin + 1, (int)startRoom.yMin + 1),
            new Vector2Int((int)startRoom.xMin + 1, (int)startRoom.yMax - 1),
            new Vector2Int((int)startRoom.xMax - 1, (int)startRoom.yMin + 1),
            new Vector2Int((int)startRoom.xMax - 1, (int)startRoom.yMax - 1),
            new Vector2Int((int)startRoom.center.x, (int)startRoom.center.y),
            new Vector2Int((int)startRoom.center.x, (int)startRoom.yMax - 1),
            new Vector2Int((int)startRoom.center.x, (int)startRoom.yMin + 1),
            new Vector2Int((int)startRoom.xMax - 1, (int)startRoom.center.y),
            new Vector2Int((int)startRoom.xMin + 1, (int)startRoom.center.y)
        };
        Vector2Int[] endRoomCoordinates =
        {
            new Vector2Int((int)endRoom.xMin + 1, (int)endRoom.yMin + 1),
            new Vector2Int((int)endRoom.xMin + 1, (int)endRoom.yMax - 1),
            new Vector2Int((int)endRoom.xMax - 1, (int)endRoom.yMin + 1),
            new Vector2Int((int)endRoom.xMax - 1, (int)endRoom.yMax - 1),
            new Vector2Int((int)endRoom.center.x, (int)endRoom.center.y),
            new Vector2Int((int)endRoom.center.x, (int)endRoom.yMax - 1),
            new Vector2Int((int)endRoom.center.x, (int)endRoom.yMin + 1),
            new Vector2Int((int)endRoom.xMax - 1, (int)endRoom.center.y),
            new Vector2Int((int)endRoom.xMin + 1, (int)endRoom.center.y)
        };
        foreach (Vector2Int startCoordinate in startRoomCoordinates)
        {
            foreach (Vector2Int endCoordinate in endRoomCoordinates)
            {
                float distance = Vector2Int.Distance(startCoordinate, endCoordinate);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCoordinateMatch[0] = startCoordinate;
                    bestCoordinateMatch[1] = endCoordinate;
                }
            }
        }
        pointsToDebug.Add(bestCoordinateMatch[0]);
        pointsToDebug.Add(bestCoordinateMatch[1]);
        linesToDebug.Add(new DebugPoint(new Vector2[] { bestCoordinateMatch[0], bestCoordinateMatch[1] }, Color.yellow));
        return bestCoordinateMatch;
    }

    private List<Vector2Int> AStar(Vector2Int start, Vector2Int end)
    {
        //AStar algorithm from start to end point
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();
        PriorityQueue<Vector2Int> frontier = new PriorityQueue<Vector2Int>();
        frontier.Enqueue(start, 0);
        cameFrom[start] = start;
        costSoFar[start] = 0;
        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == end)
            {
                break;
            }
            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (!costSoFar.ContainsKey(next) || costSoFar[next] > costSoFar[current] + 1)
                {
                    costSoFar[next] = costSoFar[current] + 1;
                    float priority = costSoFar[next] + Vector2Int.Distance(next, end);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        Vector2Int currentPath = end;
        while (currentPath != start)
        {
            path.Add(currentPath);
            currentPath = cameFrom[currentPath];
        }
        path.Add(start);
        path.Reverse();
        return path;

    }

    void PlaceLighting()
    {
        //Scatter lighting over empty areas of the map
        for (int x = 0; x < width * 2; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    if (pseudoRandom.Next(0, 100) < 50)
                    {
                        //Check if other light is nearby
                        bool nearbyLight = false;
                        foreach (Transform child in propsContainer.transform)
                        {
                            if (Vector3.Distance(child.position, new Vector3(transform.position.x + x + 0.5f, (transform.position.y + y * 2) + 0.5f, 0)) < 5)
                            {
                                nearbyLight = true;
                            }
                        }
                        //Check if it blocks a corridor check all diagonals
                        bool blocksCorridor = false;
                        if (wallTilemap.GetTile(new Vector3Int(x, y + 1, 0)) != null && wallTilemap.GetTile(new Vector3Int(x, y - 1, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 1, y, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 1, y, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 1, y + 1, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 1, y - 1, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 1, y - 1, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 1, y + 1, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 1, y + 2, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 1, y - 2, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 1, y - 2, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 1, y + 2, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 2, y + 1, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 2, y - 1, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 2, y - 1, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 2, y + 1, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 2, y + 2, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 2, y - 2, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 2, y - 2, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 2, y + 2, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x + 2, y, 0)) != null && wallTilemap.GetTile(new Vector3Int(x - 2, y, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        else if (wallTilemap.GetTile(new Vector3Int(x, y + 2, 0)) != null && wallTilemap.GetTile(new Vector3Int(x, y - 2, 0)) != null)
                        {
                            blocksCorridor = true;
                        }
                        if (!nearbyLight && !blocksCorridor)
                        {
                            Instantiate(lightPrefabs[pseudoRandom.Next(lightPrefabs.Length)], new Vector3(transform.position.x + x + 0.5f, (transform.position.y + y * 2) + 0.5f, 0), Quaternion.identity, propsContainer.transform);
                        }
                    }
                }
            }
        }
    }

    void PlaceProps()
    {
        //Place props in rooms
        foreach (Rect room in rooms.Select(r => r.room).ToList())
        {
            int propAmount = pseudoRandom.Next(decorMin, decorMax);
            for (int i = 0; i < propAmount; i++)
            {
                int x = pseudoRandom.Next((int)room.xMin + 1, (int)room.xMax - 1);
                int y = pseudoRandom.Next((int)room.yMin + 1, (int)room.yMax - 1);
                if (wallTilemap.GetTile(new Vector3Int(x, y * 2, 0)) == null)
                {
                    if (pseudoRandom.Next(0, 100) < 75)
                    {
                        walkableDecorationTilemap.SetTile(new Vector3Int(x, y * 2, 0), walkableDecorationTiles);
                    }
                    else
                    {
                        //Check if it blocks a hallway
                        if(!CheckIfInHallway(new Vector2Int(x, y * 2))) {
                            unwalkableDecorationTilemap.SetTile(new Vector3Int(x, y * 2, 0), unwalkableDecorationTiles);
                        }
                    }
                }
            }
        }
    }

    void PlaceChests()
    {
        //Get Random Rooms and place chests
        foreach (Rect room in rooms.Where(r => r.room != startRoom.room && !chestRooms.Contains(r.room)).Select(r => r.room).ToList())
        {
            if (pseudoRandom.Next(0, 100) < chestChance)
            {
                int x = (int)room.center.x;
                int y = (int)room.yMax;
                //Finding location untill there's space for it.
                while (wallTilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    y--;
                }
                linesToDebug.Add(new DebugPoint(new Vector2[] { new Vector2Int((int)room.center.x, (int)room.center.y), new Vector2Int(x, y - 1) }, Color.cyan));
                GameObject ch = Instantiate(chestPrefabs[pseudoRandom.Next(chestPrefabs.Length)], new Vector3(transform.position.x + x + 0.5f, (transform.position.y + y * 2) - 1f, 0), Quaternion.identity, propsContainer.transform);
                ch.GetComponent<Chest>().level = level;
                //Make sure the chest doesn't collide with any gameobject
                foreach (Transform prop in propsContainer.transform)
                {
                    if (ch.GetComponent<Collider2D>().IsTouching(prop.GetComponent<Collider2D>()))
                    {
                        Destroy(prop);
                        break;
                    }
                }
                rooms[rooms.FindIndex(r => r.room == room)] = new DebugRoom(room, Color.yellow);
                chestRooms.Add(room);
                chests.Add(room, ch.gameObject);
            }
        }
        //Find secluded places and place chests
        for (int x = 0; x < width * 2; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    if (wallTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null && wallTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null)
                    {
                        if (wallTilemap.GetTile(new Vector3Int(x + 1, y, 0)) == null && wallTilemap.GetTile(new Vector3Int(x - 1, y, 0)) == null)
                        {
                            if (CheckIfInHallway(new Vector2Int(x, y))) continue;
                            if (pseudoRandom.Next(0, 100) < 10)
                            {
                                bool inRoom = false;
                                //Check if this is inside a room
                                foreach (Rect room in rooms.Select(r => r.room).ToList())
                                {
                                    if (room.Contains(new Vector2(x, y)))
                                    {
                                        inRoom = true;
                                    }
                                }
                                if (!inRoom)
                                {
                                    Instantiate(chestPrefabs[pseudoRandom.Next(chestPrefabs.Length)], new Vector3(transform.position.x + x + 0.5f, (transform.position.y + y * 2) - 1f, 0), Quaternion.identity, propsContainer.transform).GetComponent<Chest>().level = level;
                                }
                            }
                        }
                    }
                }
            }
        }
        // PlaceEnemies();
    }

    public void PlaceBonfire()
    {
        //Place 1 Bonfire in the center of a random room, then find a room far away from the Bonfire and place an Exit there
        Rect room = rooms.Select(r => r.room).ToList()[pseudoRandom.Next(rooms.Count)];
        GameObject startBon = Instantiate(bonfirePrefab, new Vector3(transform.position.x + room.center.x, transform.position.y + room.center.y * 2, 0.1f), Quaternion.identity, propsContainer.transform);
        Bonfire bonfire = startBon.GetComponent<Bonfire>();
        bonfire.gameController = gameController;
        bonfire.active = true;
        bonfire.StartBonfire();
        startRoom = new DebugRoom(room, Color.green);
        rooms[rooms.FindIndex(r => r.room == startRoom.room)] = startRoom;
        Rect exitRoom = rooms.Select(r => r.room).ToList()[pseudoRandom.Next(rooms.Count)];
        while (exitRoom.center.x == room.center.x && exitRoom.center.y == room.center.y)
        {
            exitRoom = rooms.Select(r => r.room).ToList()[pseudoRandom.Next(rooms.Count)];
        }
        rooms[rooms.FindIndex(r => r.room == exitRoom)] = new DebugRoom(exitRoom, new Color(255f / 255f, 117f / 255f, 24f / 255f));
        chestRooms.Add(exitRoom);
        // Instantiate(chestPrefabs[pseudoRandom.Next(chestPrefabs.Length)], new Vector3(chestRoom.center.x, chestRoom.center.y * 2, 0), Quaternion.identity, propsContainer.transform);
        GameObject exitBon = Instantiate(exitPrefab, new Vector3(transform.position.x + exitRoom.center.x, transform.position.y + exitRoom.center.y * 2, 0.1f), Quaternion.identity, propsContainer.transform);
        Exit exit = exitBon.GetComponent<Exit>();
        chests.Add(exitRoom, exit.gameObject);
        exit.mapGenerator = this;
    }

    void PlaceEnemies()
    {
        //Scatter enemies in rooms, but not in the room with the Bonfire and if there is a chest room make the enemy level slightly higher, and their amount more
        foreach (Rect room in rooms.Select(r => r.room).ToList())
        {
            //Random color based on number
            Color teamColor = new Color(pseudoRandom.Next(0, 255) / 255f, pseudoRandom.Next(0, 255) / 255f, pseudoRandom.Next(0, 255) / 255f);
            Debug.Log(teamColor);
            if (room == startRoom.room) continue;
            if (!chestRooms.Contains(room))
            {
                int enemyAmount = pseudoRandom.Next(enemyMin, enemyMax);
                for (int i = 0; i < enemyAmount; i++)
                {
                    int x = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                    int y = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                    if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        GameObject enemy = Instantiate(enemyPrefabs[pseudoRandom.Next(enemyPrefabs.Length)], new Vector3(transform.position.x + x + 0.5f, (transform.position.y + y * 2) + 0.5f, 0), Quaternion.identity, propsContainer.transform);
                        AI2 ai = enemy.GetComponent<AI2>();
                        ai.levelRange = new Vector2Int(level + 1, level + 4);
                        //Equip the AI
                        ai.EquipArmor(gameController.CreateArmor(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.EquipWeapon(gameController.CreateWeapon(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.agressive = true;
                        ai.defaultState = AI2.AIState.Wander;
                        ai.state = AI2.AIState.Wander;
                        ai.team = rooms.FindIndex(r => r.room == room);
                        ai.gameObject.GetComponentInChildren<ShadowCaster2D>().gameObject.GetComponent<SpriteRenderer>().color = teamColor;
                        ai.wanderPoint = new Vector2(transform.position.x + room.center.x, transform.position.y + (room.center.y * 2));
                        if(pseudoRandom.Next(0, 100) < 50) {
                            ai.wanderRadius = (int)((room.size.x + room.size.y) / 2);
                        } else {
                            ai.wanderRadius = -1;
                        }
                    }
                }
            }
            else
            {
                //Find chest within room
                GameObject objectOfValue = chests[room];

                int enemyAmount = pseudoRandom.Next(enemyMin + 1, enemyMax + 2);
                for (int i = 0; i < enemyAmount; i++)
                {
                    int x = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                    int y = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                    if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        GameObject enemy = Instantiate(enemyPrefabs[pseudoRandom.Next(enemyPrefabs.Length)], new Vector3(transform.position.x + x + 0.5f, (transform.position.y + y * 2) + 0.5f, 0), Quaternion.identity, propsContainer.transform);
                        AI2 ai = enemy.GetComponent<AI2>();
                        ai.levelRange = new Vector2Int(level + 1, level + 2);
                        //Equip the AI
                        ai.EquipArmor(gameController.CreateArmor(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.EquipWeapon(gameController.CreateWeapon(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.agressive = true;
                        ai.team = rooms.FindIndex(r => r.room == room);
                        ai.gameObject.GetComponentInChildren<ShadowCaster2D>().gameObject.GetComponent<SpriteRenderer>().color = teamColor;
                        if (i == 0)
                        {
                            ai.defaultState = AI2.AIState.Guard;
                            ai.state = AI2.AIState.Guard;
                            ai.guardPoint = objectOfValue.transform.position + new Vector3(1, 0, 0);
                        }
                        else if (i == 1)
                        {
                            ai.defaultState = AI2.AIState.Guard;
                            ai.state = AI2.AIState.Guard;
                            ai.guardPoint = objectOfValue.transform.position + new Vector3(-1, 0, 0);
                        }
                        else
                        {
                            ai.defaultState = AI2.AIState.Patrol;
                            ai.state = AI2.AIState.Patrol;
                            ai.patrolPoints = new List<Vector2>();
                            ai.patrolPoints.Add(new Vector2(transform.position.x + room.xMin + 1f, transform.position.y + ((room.yMin + 1f) * 2)));
                            ai.patrolPoints.Add(new Vector2(transform.position.x + room.xMax - 1f, transform.position.y + ((room.yMin + 1f) * 2)));
                            ai.patrolPoints.Add(new Vector2(transform.position.x + room.xMax - 1f, transform.position.y + ((room.yMax - 1f) * 2)));
                            ai.patrolPoints.Add(new Vector2(transform.position.x + room.xMin + 1f, transform.position.y + ((room.yMax - 1f) * 2)));
                            ai.patrolTimer = 0f;
                            ai.patrolIndex = i % 4;
                        }
                    }
                }
            }
        }
    }

    void CleanUp(int iterations = 2)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            for (int x = 0; x < width * 2; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                    {
                        if (wallTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null && wallTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null)
                        {
                            pointsToDebug.Add(new Vector2(x, y * 2));
                            wallTilemap.SetTile(new Vector3Int(x, y + 1, 0), wallTiles);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 2, 0), roofTiles);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 3, 0), roofTiles);
                        }
                        if (
                                        wallTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == null &&
                                        wallTilemap.GetTile(new Vector3Int(x, y - 1, 0)) != null &&
                                        wallTilemap.GetTile(new Vector3Int(x + 1, y, 0)) != null &&
                                        wallTilemap.GetTile(new Vector3Int(x - 1, y, 0)) != null &&
                                        wallTilemap.GetTile(new Vector3Int(x + 1, y + 1, 0)) != null &&
                                        wallTilemap.GetTile(new Vector3Int(x - 1, y + 1, 0)) != null
                                    )
                        {
                            wallTilemap.SetTile(new Vector3Int(x, y + 1, 0), wallTiles);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 2, 0), roofTiles);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 3, 0), roofTiles);
                        }
                        if (wallTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null && wallTilemap.GetTile(new Vector3Int(x, y - 2, 0)) != null)
                        {
                            wallTilemap.SetTile(new Vector3Int(x, y - 2, 0), null);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) - 3, 0), null);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) - 4, 0), null);
                        }
                        if (wallTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == null)
                        {
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2), 0), null);
                            roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 1, 0), null);
                        }
                    }
                }
            }
        }
        PlaceLighting();
    }

    bool CheckIfInHallway(Vector2Int point)
    {
        foreach (DebugPoint debugPoint in linesToDebug)
        {
            if (debugPoint.points[0].x == point.x && debugPoint.points[0].y == point.y)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pointsToDebug != null && debugPoints)
        {
            foreach (Vector2 point in pointsToDebug)
            {
                Gizmos.DrawSphere((Vector2)transform.position + point, 0.1f);
            }
        }
        if (linesToDebug != null && debugLines)
        {
            foreach (DebugPoint point in linesToDebug)
            {
                Gizmos.color = point.color;
                if(point.points.Length == 2) {
                    Gizmos.DrawLine(new Vector2(transform.position.x + point.points[0].x, transform.position.y + point.points[0].y * 2), new Vector2(transform.position.x + point.points[1].x, transform.position.y + point.points[1].y * 2));
                } else {
                    Gizmos.DrawWireSphere(new Vector2(transform.position.x + point.points[0].x, transform.position.y + point.points[0].y * 2), 2f);
                }
            }
        }
        if (rooms != null && debugRooms)
        {
            foreach (DebugRoom room in rooms)
            {
                Gizmos.color = room.color;
                Gizmos.DrawWireCube(new Vector3(transform.position.x + room.room.center.x, transform.position.y + room.room.center.y * 2, 0), new Vector3(room.room.width, room.room.height * 2, 0));
            }
        }
    }

}

[System.Serializable]
public struct DebugPoint
{
    public Vector2[] points;
    public Color color;
    public DebugPoint(Vector2[] points, Color color)
    {
        this.points = points;
        this.color = color;
    }
}
[System.Serializable]
public struct DebugRoom
{
    public Rect room;
    public Color color;
    public DebugRoom(Rect room, Color color)
    {
        this.room = room;
        this.color = color;
    }
}

internal class PriorityQueue<T>
{
    private List<Tuple<T, float>> elements = new List<Tuple<T, float>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, float priority)
    {
        elements.Add(Tuple.Create(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item2 < elements[bestIndex].Item2)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

