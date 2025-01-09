using UnityEngine;

public struct Node
{
    public bool Walkable { get; set; }
    public NodeType Type { get; set; }

    public Node(bool walkable, NodeType type)
    {
        Walkable = walkable;
        Type = type;
    }
}

public enum NodeType
{
    Empty,
    Room,
    Corridor
}

public class WorldGrid
{
    public int Width => width;
    public int Height => height;
    public Node[,] WalkableGrid => walkableGrid;

    private Node[,] walkableGrid;
    private int width;
    private int height;

    public WorldGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        walkableGrid = new Node[width, height];
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool IsWalkable(int x, int y)
    {
        return IsInBounds(x, y) && walkableGrid[x, y].Walkable;
    }

    public bool IsType(int x, int y, NodeType type)
    {
        return IsInBounds(x, y) && walkableGrid[x, y].Type == type;
    }

    public void SetWalkable(int x, int y, bool value, NodeType type)
    {
        if (IsInBounds(x, y))
        {
            walkableGrid[x, y] = new Node(value, type);
        }
    }

    // ----------------------------------------------------------------
    // FLOORS
    // ----------------------------------------------------------------
    public void InstantiateFloors(GameObject voxelPrefab, Transform parent)
    {
        if (voxelPrefab == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (walkableGrid[x, y].Walkable)
                {
                    // Put a floor voxel at y=0
                    Object.Instantiate(
                        voxelPrefab,
                        new Vector3(x, 0f, y),
                        Quaternion.identity,
                        parent
                    );
                }
            }
        }
    }

    // ----------------------------------------------------------------
    // WALLS
    // ----------------------------------------------------------------
    public void InstantiateWalls(GameObject voxelPrefab, Transform parent, float roofHeight = 5f)
    {
        if (voxelPrefab == null) return;

        // Directions: left, right, down, up
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1,  0), // Left
            new Vector2Int( 1,  0), // Right
            new Vector2Int( 0, -1), // Down
            new Vector2Int( 0,  1), // Up
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node currentNode = walkableGrid[x, y];

                // Handle Walls for Rooms
                if (currentNode.Type == NodeType.Room)
                {
                    foreach (var dir in directions)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;

                        if (!IsInBounds(nx, ny) || walkableGrid[nx, ny].Type == NodeType.Empty)
                        {
                            for (float h = 1f; h < roofHeight; h += 1f)
                            {
                                Vector3 wallPos = new Vector3(
                                    x + dir.x * 0.5f,
                                    h,
                                    y + dir.y * 0.5f
                                );

                                Object.Instantiate(voxelPrefab, wallPos, Quaternion.identity, parent);
                            }
                        }
                    }
                }
                // Handle Walls for Corridors
                else if (currentNode.Type == NodeType.Corridor)
                {
                    bool hasLeft = IsInBounds(x - 1, y) && walkableGrid[x - 1, y].Type == NodeType.Corridor;
                    bool hasRight = IsInBounds(x + 1, y) && walkableGrid[x + 1, y].Type == NodeType.Corridor;
                    bool hasDown = IsInBounds(x, y - 1) && walkableGrid[x, y - 1].Type == NodeType.Corridor;
                    bool hasUp = IsInBounds(x, y + 1) && walkableGrid[x, y + 1].Type == NodeType.Corridor;

                    bool isHorizontal = hasLeft || hasRight;
                    bool isVertical = hasDown || hasUp;

                    if (isHorizontal)
                    {
                        PlaceWall(voxelPrefab, parent, x, y, "Up", roofHeight);
                        PlaceWall(voxelPrefab, parent, x, y, "Down", roofHeight);
                    }

                    if (isVertical)
                    {
                        PlaceWall(voxelPrefab, parent, x, y, "Left", roofHeight);
                        PlaceWall(voxelPrefab, parent, x, y, "Right", roofHeight);
                    }
                }
            }
        }
    }

    // ----------------------------------------------------------------
    // ROOFS
    // ----------------------------------------------------------------
    public void InstantiateRoofs(GameObject voxelPrefab, Transform parent, float roofHeight = 5f)
    {
        if (voxelPrefab == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (walkableGrid[x, y].Walkable)
                {
                    Object.Instantiate(
                        voxelPrefab,
                        new Vector3(x, roofHeight, y),
                        Quaternion.identity,
                        parent
                    );
                }
            }
        }
    }

    private void PlaceWall(GameObject voxelPrefab, Transform parent, int x, int y, string direction, float roofHeight)
    {
        Vector3 wallPos = Vector3.zero;

        switch (direction)
        {
            case "Up":
                wallPos = new Vector3(x, 1f, y + 0.5f);
                break;
            case "Down":
                wallPos = new Vector3(x, 1f, y - 0.5f);
                break;
            case "Left":
                wallPos = new Vector3(x - 0.5f, 1f, y);
                break;
            case "Right":
                wallPos = new Vector3(x + 0.5f, 1f, y);
                break;
        }

        // Determine the grid position corresponding to the wall's direction
        int neighborX = x;
        int neighborY = y;

        switch (direction)
        {
            case "Up":
                neighborY += 1;
                break;
            case "Down":
                neighborY -= 1;
                break;
            case "Left":
                neighborX -= 1;
                break;
            case "Right":
                neighborX += 1;
                break;
        }

        // Check if the neighbor is Empty before placing the wall
        if (IsInBounds(neighborX, neighborY) && walkableGrid[neighborX, neighborY].Type != NodeType.Empty)
        {
            // Neighbor is not Empty; do not place wall to avoid blocking corridors connected to rooms
            return;
        }

        // Stack wall voxels from y=1 up to roofHeight - 1
        for (float h = 1f; h < roofHeight; h += 1f)
        {
            Vector3 currentWallPos = new Vector3(
                wallPos.x,
                h,
                wallPos.z
            );

            // Instantiate the wall voxel
            Object.Instantiate(voxelPrefab, currentWallPos, Quaternion.identity, parent);
        }
    }
}
