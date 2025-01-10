using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("World Settings")]
    public int gridWidth = 100;
    public int gridHeight = 100;
    public int numberOfRooms = 5;
    public Vector2Int roomSizeMin = new Vector2Int(5, 5);
    public Vector2Int roomSizeMax = new Vector2Int(10, 10);

    [Header("Voxel Prefab (1x1x1)")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject roofPrefab;

    [Header("Wall / Roof Settings")]
    public float roofHeight = 5f;

    public GameObject enemySpawnerPrefab;
    public GameObject playerPrefab;

    private WorldGrid grid;
    private List<RoomData> rooms;
    private List<GraphMST.Edge> mstEdges;

    void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 pos = new Vector3(x, 0.1f, y);
                switch (grid.WalkableGrid[x, y].Type)
                {
                    case NodeType.Room:
                        Gizmos.color = Color.blue;
                        break;
                    case NodeType.Corridor:
                        Gizmos.color = Color.green;
                        break;
                    default:
                        continue;
                }
                Gizmos.DrawCube(pos, Vector3.one * 0.9f);
            }
        }
    }

    void Start()
    {
        GenerateWorld();
        SpawnPlayerAndSpawners();
    }

    private void GenerateWorld()
    {
        // Create random rooms
        rooms = Enumerable.Range(0, numberOfRooms)
            .Aggregate(new List<RoomData>(), (acc, _) => {
              while (true)
              {
                  var room = RoomData.NewRoomWithin(roomSizeMin, roomSizeMax, gridWidth, gridHeight);
                  if (acc.Any(r => r.Intersects(room))) continue;
                  acc.Add(room);
                  break;
              }
              return acc;
            })
            .ToList();

        grid = new WorldGrid(gridWidth, gridHeight);

        // Mark rooms
        MarkRoomsOnGrid();

        // MST + BFS carve
        mstEdges = GraphMST.CreateMST(rooms);
        foreach (var edge in mstEdges)
        {
            RoomData rA = rooms[edge.roomA];
            RoomData rB = rooms[edge.roomB];
            BFS.CarvePath(rA.position, rB.position, grid, true);
        }

        // Floors
        grid.InstantiateFloors(floorPrefab, transform);

        // Walls (now referencing roofHeight so they meet the roof)
        grid.InstantiateWalls(wallPrefab, transform, roofHeight);

        // Roofs
        grid.InstantiateRoofs(roofPrefab, transform, roofHeight);
    }

    private void MarkRoomsOnGrid()
    {
        foreach (var room in rooms)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    if (grid.IsInBounds(x, y))
                    {
                        grid.SetWalkable(x, y, true, NodeType.Room);
                    }
                }
            }
        }
    }

    private void SpawnPlayerAndSpawners()
    {
        if (rooms == null || rooms.Count == 0) return;

        // Player
        int randomIndex = Random.Range(0, rooms.Count);
        RoomData randomRoom = rooms[randomIndex];
        Vector3 playerPos = new Vector3(randomRoom.position.x, 1.5f, randomRoom.position.y);
        if (playerPrefab != null)
        {
            playerPrefab.transform.position = playerPos;
        }

        // Spawner
        foreach (RoomData room in rooms)
        {
            if (enemySpawnerPrefab != null)
            {
                Vector3 spawnerPos = new Vector3(room.position.x, 1.5f, room.position.y);
                Instantiate(enemySpawnerPrefab, spawnerPos, Quaternion.identity);
            }
        }
    }
}
