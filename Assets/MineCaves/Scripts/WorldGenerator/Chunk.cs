using System.Collections.Generic;
using UnityEngine;

public struct CellContent
{
    public MiningTile liquid;
    public MiningTile passableFront;
    public MiningTile passableBack;
    public MiningTile solid;
}

public struct PrefabState
{
    public BlockData blockData;
    public int durability;
}

public class Chunk
{
    public ChunkCord chunkCord;
    public bool wasChanged = false;

    private Dictionary<Vector2Int, CellContent> overrides = new Dictionary<Vector2Int, CellContent>();
    private Dictionary<Vector3Int, PrefabState> prefabStates = new Dictionary<Vector3Int, PrefabState>();

    public Dictionary<Vector2Int, CellContent> Overrides
    {
        get { return overrides; }
    }

    public Dictionary<Vector3Int, PrefabState> PrefabStates
    {
        get { return prefabStates; }
    }

    public void SetCell(Vector2Int localPosition, CellContent content)
    {
        overrides[localPosition] = content;
        wasChanged = true;
    }

    public void SetPrefabState(Vector3Int key, PrefabState state)
    {
        prefabStates[key] = state;
        wasChanged = true;
    }

    public void RemovePrefabState(Vector3Int key)
    {
        if (prefabStates.ContainsKey(key))
        {
            prefabStates.Remove(key);
            wasChanged = true;
        }
    }


    public static Vector3Int MakePrefabKey(Vector2Int localPosition, TileLayer layer)
    {
        return new Vector3Int(localPosition.x, localPosition.y, (int)layer);
    }
}
