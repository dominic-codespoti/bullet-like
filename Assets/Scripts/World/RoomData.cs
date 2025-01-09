using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int roomID;
    public Vector2Int position;
    public Vector2Int size;
    
    public int xMin => position.x - size.x / 2;
    public int xMax => position.x + size.x / 2;
    public int yMin => position.y - size.y / 2;
    public int yMax => position.y + size.y / 2;

    public RoomData(int id, Vector2Int pos, Vector2Int s)
    {
        roomID = id;
        position = pos;
        size = s;
    }

    public static RoomData NewRoomWithin(Vector2Int min, Vector2Int max, int gridWidth, int gridHeight)
    {
        int w = Random.Range(min.x, max.x);
        int h = Random.Range(min.y, max.y);
        Vector2Int size = new Vector2Int(w, h);
        Vector2Int pos = new Vector2Int(
            Random.Range(size.x / 2, gridWidth - size.x / 2),
            Random.Range(size.y / 2, gridHeight - size.y / 2)
        );
        return new RoomData(-1, pos, size);
    }

    public bool Intersects(RoomData other)
    {
        return xMin < other.xMax && xMax > other.xMin && yMin < other.yMax && yMax > other.yMin;
    }
}
