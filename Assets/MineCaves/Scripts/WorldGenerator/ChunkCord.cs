using System;
using UnityEngine;

public class ChunkCord
{
    public const int chunkSize = 16;
    public int x;
    public int y;

    public ChunkCord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static ChunkCord worldToCell(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new ChunkCord(x, y);
    }

    public Vector3Int LocalToWorld(Vector2Int localPosition)
    {
        return new Vector3Int(x * chunkSize + localPosition.x, y * chunkSize + localPosition.y, 0);
    }

    public Vector3Int LocalToWorld(int localX, int localY)
    {
        return new Vector3Int(x * chunkSize + localX, y * chunkSize + localY, 0);
    }

    public override bool Equals(object obj)
    {
        if(obj is ChunkCord chunkCord)
        {
          return this.x == chunkCord.x && this.y == chunkCord.y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static bool operator ==(ChunkCord a, ChunkCord b)
    {
        if(ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;
        if(ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(ChunkCord a, ChunkCord b)
    {
        return !(a == b);
    }
}
