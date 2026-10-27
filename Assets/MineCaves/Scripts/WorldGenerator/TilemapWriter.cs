using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileLayer
{
    Solid,
    PassableBack,
    PassableFront,
    Liquid,
    Background
}

public class TilemapWriter : MonoBehaviour
{
    [SerializeField] private Tilemap solid;
    [SerializeField] private Tilemap passableBack;
    [SerializeField] private Tilemap passableFront;
    [SerializeField] private Tilemap liquid;
    [SerializeField] private Tilemap background;

    public void PlaceTile(Vector3Int worldPos, TileLayer layer, MiningTile tile)
    {
        GetTilemap(layer).SetTile(worldPos, tile);
    }

    public void ChangeTile(Vector3Int worldPos, TileLayer layer, MiningTile newTile)
    {
        GetTilemap(layer).SetTile(worldPos, newTile);
        WorldEvents.CallTileChanged(worldPos, layer, newTile);
    }

    public MiningTile GetTile(Vector3Int worldPos, TileLayer layer)
    {
        return (MiningTile)GetTilemap(layer).GetTile(worldPos);
    }
    private Tilemap GetTilemap(TileLayer layer)
    {
        switch (layer)
        {
            case TileLayer.Solid: return solid;
            case TileLayer.PassableBack: return passableBack;
            case TileLayer.PassableFront: return passableFront;
            case TileLayer.Liquid: return liquid;
            case TileLayer.Background: return background;
            default: return null;
        }
    }
}
