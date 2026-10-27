using System;
using UnityEngine;

public static class WorldEvents 
{
    public static event Action<Vector3Int, TileLayer, MiningTile> TileChanged;
    public static event Action<Vector3Int, TileLayer, PrefabState> BlockStateChanged;
    public static event Action<Vector3Int, TileLayer> BlockDestroyed;

    public static void CallTileChanged(Vector3Int cell, TileLayer layer, MiningTile tile)
    {
        TileChanged?.Invoke(cell, layer, tile);
    }

    public static void CallBlockStateChanged(Vector3Int cell, TileLayer layer, PrefabState state)
    {
        BlockStateChanged?.Invoke(cell, layer, state);
    }

    public static void CallBlockDestroyed(Vector3Int cell, TileLayer layer)
    {
        BlockDestroyed?.Invoke(cell, layer);
    }
}
