using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
    /// <summary>
    /// A BFS that can move through the entire grid (regardless of walkable or not)
    /// just to find *a path* from start to goal. Once found, it carves that path
    /// (and widens if desired).
    /// </summary>
    public static void CarvePath(Vector2Int start, Vector2Int goal, WorldGrid grid, bool widen = false)
    {
        if (!grid.IsInBounds(start.x, start.y) || !grid.IsInBounds(goal.x, goal.y))
            return;

        int width  = grid.Width;
        int height = grid.Height;

        bool[,] visited = new bool[width, height];
        Vector2Int[,] parent = new Vector2Int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                parent[x, y] = new Vector2Int(-1, -1);
            }
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        Vector2Int[] directions = {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };

        bool foundGoal = false;
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == goal)
            {
                foundGoal = true;
                break;
            }

            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (!grid.IsInBounds(neighbor.x, neighbor.y))
                    continue;
                if (visited[neighbor.x, neighbor.y])
                    continue;

                visited[neighbor.x, neighbor.y] = true;
                parent[neighbor.x, neighbor.y] = current;
                queue.Enqueue(neighbor);
            }
        }

        if (!foundGoal) return;

        List<Vector2Int> path = new List<Vector2Int>();
        {
            Vector2Int cur = goal;
            while (cur != start && cur.x != -1 && cur.y != -1)
            {
                path.Add(cur);
                cur = parent[cur.x, cur.y];
            }
            path.Add(start);
            path.Reverse();
        }

        foreach (var cell in path)
        {
          if (!grid.IsType(cell.x, cell.y, NodeType.Room))
          {
              grid.SetWalkable(cell.x, cell.y, true, NodeType.Corridor);
          }
        }

        if (widen)
        {
            foreach (var cell in path)
            {
                foreach (var dir in directions)
                {
                    int nx = cell.x + dir.x;
                    int ny = cell.y + dir.y;
                    if (grid.IsInBounds(nx, ny) && !grid.IsType(nx, ny, NodeType.Room))
                    {
                        grid.SetWalkable(nx, ny, true, NodeType.Corridor);
                    }
                }
            }
        }
    }
}
