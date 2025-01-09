using System.Collections.Generic;
using UnityEngine;

public static class GraphMST
{
    public class Edge
    {
        public int roomA;
        public int roomB;
        public float weight;

        public Edge(int a, int b, float w)
        {
            roomA = a;
            roomB = b;
            weight = w;
        }
    }

    public static List<Edge> CreateMST(List<RoomData> rooms)
    {
        List<Edge> edges = new List<Edge>();
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                float weight = Vector2Int.Distance(rooms[i].position, rooms[j].position);
                edges.Add(new Edge(i, j, weight));
            }
        }

        edges.Sort((a, b) => a.weight.CompareTo(b.weight));

        DisjointSet ds = new DisjointSet(rooms.Count);
        List<Edge> mst = new List<Edge>();

        foreach (var edge in edges)
        {
            int rootA = ds.Find(edge.roomA);
            int rootB = ds.Find(edge.roomB);

            if (rootA != rootB)
            {
                mst.Add(edge);
                ds.Union(rootA, rootB);
            }
        }

        return mst;
    }

    public class DisjointSet
    {
        private int[] parent;
        private int[] rank;

        public DisjointSet(int size)
        {
            parent = new int[size];
            rank = new int[size];
            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        public int Find(int i)
        {
            if (parent[i] != i)
            {
                parent[i] = Find(parent[i]);
            }
            return parent[i];
        }

        public void Union(int i, int j)
        {
            int rootI = Find(i);
            int rootJ = Find(j);
            if (rootI != rootJ)
            {
                if (rank[rootI] > rank[rootJ])
                {
                    parent[rootJ] = rootI;
                }
                else if (rank[rootI] < rank[rootJ])
                {
                    parent[rootI] = rootJ;
                }
                else
                {
                    parent[rootJ] = rootI;
                    rank[rootI]++;
                }
            }
        }
    }
}
