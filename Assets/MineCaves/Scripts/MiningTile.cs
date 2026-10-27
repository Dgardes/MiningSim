using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "MiningTile", menuName = "Tiles/MiningTile")]
public class MiningTile : Tile
{
    [SerializeField] private BlockData blockData;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if (blockData != null)
        {
            tileData.sprite = blockData.blockSprite;
        }   
    }

    public BlockData getBlockData() 
    { 
        return blockData;
    }
}
