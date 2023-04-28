using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using NavMeshPlus.Components;

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
    public bool useRandomSeed;
    public int minRoomSize;
    public int maxRoomSize;
    public int minRooms;
    public int maxRooms;
    public int minRoomDistance;
    public int maxRoomDistance;
    public int level = 0;
    System.Random pseudoRandom;
    public int agentAmount;
    public int walkMin;
    public int walkMax;
    public int enemyMin;
    public int enemyMax;
    [Header("Decoration Settings")]
    public int decorMin;
    public int decorMax;
    [Header("Prefabs")]
    public GameObject propsContainer;
    public GameObject[] lightPrefabs;
    public GameObject[] enemyPrefabs;
    public GameObject[] chestPrefabs;
    public GameObject bonfirePrefab;
    public GameObject exitPrefab;

    [Header("Debug")]
    public bool debugRooms;
    public List<Rect> rooms = new List<Rect>();
    public List<Rect> chestRooms = new List<Rect>();
    public List<Rect[]> hallways = new List<Rect[]>();
    public bool debugPoints;
    public List<Vector2> pointsToDebug = new List<Vector2>();
    public bool debugLines;
    public List<Vector2[]> linesToDebug = new List<Vector2[]>();

    public void GenerateMap()
    {
        pointsToDebug.Clear();
        linesToDebug.Clear();
        chestRooms.Clear();
        hallways.Clear();
        rooms.Clear();
        if (useRandomSeed)
        {
            seed = UnityEngine.Random.Range(0, 1000000);
        }
        pseudoRandom = new System.Random((seed + level).GetHashCode());
        ClearMap();
        StartCoroutine(CreateRooms());
    }

    public void ClearMap()
    {
        DestroyImmediate(propsContainer);
        propsContainer = new GameObject("Props");
        propsContainer.transform.parent = transform;
        wallTilemap.ClearAllTiles();
        roofTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
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

    public IEnumerator CreateRooms()
    {
        rooms.Clear();
        int roomCount = pseudoRandom.Next(minRooms, maxRooms);
        for (int i = 0; i < roomCount; i++)
        {
            int roomWidth = pseudoRandom.Next(minRoomSize, maxRoomSize);
            int roomHeight = pseudoRandom.Next(minRoomSize / 2, maxRoomSize / 2);
            int roomX = pseudoRandom.Next(2, (width * 2) - 2 - roomWidth);
            int roomY = pseudoRandom.Next(2, height - 2 - roomHeight);
            Rect room = new Rect(roomX, roomY, roomWidth, roomHeight);
            bool overlaps = false;
            foreach (Rect otherRoom in rooms)
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
                rooms.Add(room);
            }
        }
        foreach (Rect room in rooms)
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
        List<Rect> MSPRooms = new List<Rect>(this.rooms);
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
                linesToDebug.Add(new Vector2[] { previousTile, tile });
                wallTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x, tile.y * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x, (tile.y * 2) + 1, 0), null);
                wallTilemap.SetTile(new Vector3Int(tile.x + 1, tile.y, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x + 1, tile.y * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(tile.x + 1, (tile.y * 2) + 1, 0), null);
                previousTile = tile;
            }
        }

        StartCoroutine(CleanUp());
        StartCoroutine(PlaceProps());
        PlaceBonfire();
        StartCoroutine(PlaceChests());
        yield return null;
    }

    private Vector2Int[] BestCoordinateMatch(Rect startRoom, Rect endRoom)
    {
        Vector2Int[] bestCoordinateMatch = new Vector2Int[2];
        float bestDistance = float.MaxValue;
        Vector2Int[] startRoomCoordinates =
        {
            // new Vector2Int((int)startRoom.xMin, (int)startRoom.yMin),
            // new Vector2Int((int)startRoom.xMin, (int)startRoom.yMax),
            // new Vector2Int((int)startRoom.xMax, (int)startRoom.yMin),
            // new Vector2Int((int)startRoom.xMax, (int)startRoom.yMax),
            new Vector2Int((int)startRoom.x, (int)startRoom.y),
            new Vector2Int((int)startRoom.x, (int)startRoom.yMax),
            new Vector2Int((int)startRoom.x, (int)startRoom.yMin),
            new Vector2Int((int)startRoom.xMax, (int)startRoom.y),
            new Vector2Int((int)startRoom.xMin, (int)startRoom.y)
        };
        Vector2Int[] endRoomCoordinates =
        {
            // new Vector2Int((int)endRoom.xMin, (int)endRoom.yMin),
            // new Vector2Int((int)endRoom.xMin, (int)endRoom.yMax),
            // new Vector2Int((int)endRoom.xMax, (int)endRoom.yMin),
            // new Vector2Int((int)endRoom.xMax, (int)endRoom.yMax),
            new Vector2Int((int)endRoom.x, (int)endRoom.y),
            new Vector2Int((int)endRoom.x, (int)endRoom.yMax),
            new Vector2Int((int)endRoom.x, (int)endRoom.yMin),
            new Vector2Int((int)endRoom.xMax, (int)endRoom.y),
            new Vector2Int((int)endRoom.xMin, (int)endRoom.y)
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
        linesToDebug.Add(new Vector2[] { bestCoordinateMatch[0], bestCoordinateMatch[1] });
        return bestCoordinateMatch;
    }

    private List<Vector2Int> AStar(Vector2Int start, Vector2Int end)
    {
        //AStart algorithm from start to end point
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

    IEnumerator PlaceLighting()
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
                            if (Vector3.Distance(child.position, new Vector3(x + 0.5f, (y * 2) + 0.5f, 0)) < 5)
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
                            Instantiate(lightPrefabs[pseudoRandom.Next(lightPrefabs.Length)], new Vector3(x + 0.5f, (y * 2) + 0.5f, 0), Quaternion.identity, propsContainer.transform);
                        }
                    }
                }
            }
        }
        yield return null;
    }

    IEnumerator PlaceProps()
    {
        //Place props in rooms
        foreach (Rect room in rooms)
        {
            int propAmount = pseudoRandom.Next(decorMin, decorMax);
            for (int i = 0; i < propAmount; i++)
            {
                int x = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                int y = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    if (pseudoRandom.Next(0, 100) < 50)
                    {
                        walkableDecorationTilemap.SetTile(new Vector3Int(x, y, 0), walkableDecorationTiles);
                    }
                    else
                    {
                        unwalkableDecorationTilemap.SetTile(new Vector3Int(x, y, 0), unwalkableDecorationTiles);
                    }
                }
            }
        }
        yield return null;
    }

    IEnumerator PlaceChests() {
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
                            if (pseudoRandom.Next(0, 100) < 10)
                            {
                                //Check if this is inside a room and if so put the room in chestRooms
                                Rect insideRoom = new Rect();
                                bool insideRoomCheck = false;
                                bool allowed = true;
                                foreach (Rect room in rooms)
                                {
                                    if (room.Contains(new Vector2(x, y)))
                                    {
                                        if(!chestRooms.Contains(room)) {
                                            insideRoomCheck = true;
                                            insideRoom = room;
                                            chestRooms.Add(room);
                                            linesToDebug.Add(new Vector2[] { room.center, new Vector2(insideRoom.center.x + 0.5f, (insideRoom.yMax) - 1f) });
                                        } else {
                                            allowed = false;
                                        }
                                    }
                                }
                                if(!allowed) continue;
                                if(insideRoomCheck) {
                                    Instantiate(chestPrefabs[pseudoRandom.Next(chestPrefabs.Length)], new Vector3(insideRoom.center.x + 0.5f, (insideRoom.yMax * 2) - 1f, 0), Quaternion.identity, propsContainer.transform);
                                } else {
                                    Instantiate(chestPrefabs[pseudoRandom.Next(chestPrefabs.Length)], new Vector3(x + 0.5f, (y * 2) - 1f, 0), Quaternion.identity, propsContainer.transform);
                                }
                            }
                        }
                    }
                }
            }
        }
        GameObject.FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        StartCoroutine(PlaceEnemies());
        yield return null;
    }

    public void PlaceBonfire() {
        //Place 1 Bonfire in the center of a random room, then find a room far away from the Bonfire and place an Exit there
        Rect room = rooms[pseudoRandom.Next(rooms.Count)];
        rooms.Remove(room);
        Instantiate(bonfirePrefab, new Vector3(room.center.x, room.center.y * 2, 0.1f), Quaternion.identity, propsContainer.transform);
        Rect exitRoom = rooms[pseudoRandom.Next(rooms.Count)];
        while (exitRoom.center.x == room.center.x && exitRoom.center.y == room.center.y) {
            exitRoom = rooms[pseudoRandom.Next(rooms.Count)];
        }
        chestRooms.Add(exitRoom);
        // Instantiate(chestPrefabs[pseudoRandom.Next(chestPrefabs.Length)], new Vector3(chestRoom.center.x, chestRoom.center.y * 2, 0), Quaternion.identity, propsContainer.transform);
        Instantiate(exitPrefab, new Vector3(exitRoom.center.x, exitRoom.center.y * 2, 0.1f), Quaternion.identity, propsContainer.transform);
    }

    IEnumerator PlaceEnemies() {
        //Scatter enemies in rooms, but not in the room with the Bonfire and if there is a chest room make the enemy level slightly higher, and their amount more
        foreach (Rect room in rooms)
        {
            if (!chestRooms.Contains(room))
            {
                int enemyAmount = pseudoRandom.Next(enemyMin, enemyMax);
                for (int i = 0; i < enemyAmount; i++)
                {
                    int x = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                    int y = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                    if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        GameObject enemy = Instantiate(enemyPrefabs[pseudoRandom.Next(enemyPrefabs.Length)], new Vector3(x + 0.5f, (y * 2) + 0.5f, 0), Quaternion.identity, propsContainer.transform);
                        AI2 ai = enemy.GetComponent<AI2>();
                        ai.levelRange = new Vector2Int(level + 1, level + 4);
                        //Equip the AI
                        ai.EquipArmor(GameController.GetInstance().CreateArmor(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.EquipWeapon(GameController.GetInstance().CreateWeapon(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.agressive = true;
                    }
                }
            }
            else
            {
                int enemyAmount = pseudoRandom.Next(enemyMin + 1, enemyMax + 2);
                for (int i = 0; i < enemyAmount; i++)
                {
                    int x = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                    int y = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                    if (wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                    {
                        GameObject enemy = Instantiate(enemyPrefabs[pseudoRandom.Next(enemyPrefabs.Length)], new Vector3(x + 0.5f, (y * 2) + 0.5f, 0), Quaternion.identity, propsContainer.transform);
                        AI2 ai = enemy.GetComponent<AI2>();
                        ai.levelRange = new Vector2Int(level + 1, level + 2);
                        //Equip the AI
                        ai.EquipArmor(GameController.GetInstance().CreateArmor(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.EquipWeapon(GameController.GetInstance().CreateWeapon(GameController.GetRarity(), pseudoRandom.Next((int)ai.levelRange.x, (int)ai.levelRange.y), pseudoRandom));
                        ai.agressive = true;
                    }
                }
            }
        }
        yield return null;
    }

    IEnumerator CleanUp(int iterations = 2)
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
        StartCoroutine(PlaceLighting());
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(pointsToDebug != null && debugPoints) {
            foreach (Vector2 point in pointsToDebug)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
        if(linesToDebug != null && debugLines) {
            foreach (Vector2[] line in linesToDebug)
            {
                Gizmos.DrawLine(new Vector2(line[0].x, line[0].y * 2), new Vector2(line[1].x, line[1].y * 2));
            }
        }
        if(rooms != null && debugRooms) {
            foreach (Rect room in rooms)
            {
                Gizmos.DrawWireCube(new Vector3(room.center.x, room.center.y * 2, 0), new Vector3(room.width, room.height * 2, 0));
            }
        }
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