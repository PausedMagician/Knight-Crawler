using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap wallTilemap;
    public Tilemap roofTilemap;
    public RuleTile wallTiles;
    public RuleTile roofTiles;
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

    private void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        if(useRandomSeed) {
            seed = Random.Range(0, 1000000);
        }
        pseudoRandom = new System.Random((seed + level).GetHashCode());
        ClearMap();
        CreateRooms();
    }

    public void ClearMap() {
        wallTilemap.ClearAllTiles();
        roofTilemap.ClearAllTiles();
        for(int x = 0; x < width * 2; x++) {
            for(int y = 0; y < height; y++) {
                wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTiles);
            }
            for(int y = 0; y < height * 2; y++) {
                roofTilemap.SetTile(new Vector3Int(x, y, 0), roofTiles);
            }
        }
    }

    public void CreateRooms() {
        List<Rect> rooms = new List<Rect>();
        int roomCount = pseudoRandom.Next(minRooms, maxRooms);
        for(int i = 0; i < roomCount; i++) {
            int roomWidth = pseudoRandom.Next(minRoomSize, maxRoomSize);
            int roomHeight = pseudoRandom.Next(minRoomSize/2, maxRoomSize/2);
            int roomX = pseudoRandom.Next(2, (width * 2) - 2 - roomWidth);
            int roomY = pseudoRandom.Next(2, height - 2 - roomHeight);
            Rect room = new Rect(roomX, roomY, roomWidth, roomHeight);
            bool overlaps = false;
            foreach(Rect otherRoom in rooms) {
                Rect sizedUpRoom = otherRoom;
                sizedUpRoom.xMin -= minRoomDistance;
                sizedUpRoom.xMax += minRoomDistance;
                sizedUpRoom.yMin -= minRoomDistance;
                sizedUpRoom.yMax += minRoomDistance;
                if(room.Overlaps(sizedUpRoom)) {
                    overlaps = true;
                    break;
                }
            }
            if(!overlaps) {
                rooms.Add(room);
            }
        }
        foreach(Rect room in rooms) {
            //Walls are 2 tiles tall and 1 tile wide
            for(int x = (int)room.xMin; x < room.xMax; x++) {
                for(int y = (int)room.yMin; y < room.yMax; y++) {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x, y * 2, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x, (y * 2) + 1, 0), null);
                }
            }
            //Remove ceiling that's 2 above rooms upper limit
            for(int x = (int)room.xMin; x < room.xMax; x++) {
                roofTilemap.SetTile(new Vector3Int(x, (int)room.yMax * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(x, ((int)room.yMax * 2) + 1, 0), null);
            }
            //Do random walk remove tiles around room
            for (int agent = 0; agent < agentAmount; agent++)
            {
                int walkLength = pseudoRandom.Next(walkMin, walkMax);
                int walkX = pseudoRandom.Next((int)room.xMin, (int)room.xMax);
                int walkY = pseudoRandom.Next((int)room.yMin, (int)room.yMax);
                for(int i = 0; i < walkLength; i++) {
                    int direction = pseudoRandom.Next(0, 4);
                    switch(direction) {
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
                    if(walkX < 2 || walkX >= (width * 2) - 2 || walkY < 2 || walkY >= height - 2) {
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
        minimumSpanTree.Add(rooms[0]);
        rooms.RemoveAt(0);
        while(rooms.Count > 0) {
            float closestDistance = float.MaxValue;
            Rect closestRoom = new Rect();
            Rect closestMinimumSpanTreeRoom = new Rect();
            foreach(Rect room in rooms) {
                foreach(Rect minimumSpanTreeRoom in minimumSpanTree) {
                    float distance = Vector2.Distance(new Vector2(room.x, room.y), new Vector2(minimumSpanTreeRoom.x, minimumSpanTreeRoom.y));
                    if(distance < closestDistance) {
                        closestDistance = distance;
                        closestRoom = room;
                        closestMinimumSpanTreeRoom = minimumSpanTreeRoom;
                    }
                }
            }
            //Remove tiles between rooms making sure to make paths 2 tiles wide
            int x1 = (int)closestRoom.x;
            int y1 = (int)closestRoom.y;
            int x2 = (int)closestMinimumSpanTreeRoom.x;
            int y2 = (int)closestMinimumSpanTreeRoom.y;
            bool leftExpand = (x1 < x2);
            bool upExpand = (y1 < y2);
            while(x1 != x2 || y1 != y2) {
                if(x1 != x2) {
                    if(x1 < x2) {
                        x1++;
                    } else {
                        x1--;
                    }
                }
                if(y1 != y2) {
                    if(y1 < y2) {
                        y1++;
                    } else {
                        y1--;
                    }
                }
                if(leftExpand) {
                    wallTilemap.SetTile(new Vector3Int(x1 - 1, y1, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x1 - 1, y1 * 2, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x1 - 1, (y1 * 2) + 1, 0), null);
                } else {
                    wallTilemap.SetTile(new Vector3Int(x1 + 1, y1, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x1 + 1, y1 * 2, 0), null);
                    roofTilemap.SetTile(new Vector3Int(x1 + 1, (y1 * 2) + 1, 0), null);
                }
                wallTilemap.SetTile(new Vector3Int(x1, y1, 0), null);
                roofTilemap.SetTile(new Vector3Int(x1, y1 * 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(x1, (y1 * 2) + 1, 0), null);
                //Remove extra ceiling
                roofTilemap.SetTile(new Vector3Int(x1, (y1 * 2) + 2, 0), null);
                roofTilemap.SetTile(new Vector3Int(x1, (y1 * 2) + 3, 0), null);
            }
            minimumSpanTree.Add(closestRoom);
            rooms.Remove(closestRoom);
        }

    }

}
