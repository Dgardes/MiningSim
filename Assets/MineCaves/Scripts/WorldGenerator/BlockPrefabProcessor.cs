using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockPrefabProcessor :MonoBehaviour
{
    public static BlockPrefabProcessor instance;

    [Header("Prefabs")]
    [SerializeField] private BlockLogic solidPrefab;
    [SerializeField] private BlockLogic passableBackPrefab;
    [SerializeField] private BlockLogic passableFrontPrefab;

    [Header("Parents")]
    [SerializeField] private Transform solid;
    [SerializeField] private Transform passableFront;
    [SerializeField] private Transform passableBack;

    [SerializeField] private Grid grid;
    [SerializeField] private TilemapWriter tilemapWriter;

    public Dictionary<(Vector3Int, TileLayer), BlockLogic> activeBlocks = new Dictionary<(Vector3Int, TileLayer), BlockLogic>();

    private void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        { Destroy(gameObject); }
    }

    public BlockLogic spawnBlockPrefab(Vector3Int worldPos, TileLayer layer, PrefabState state)
    {
        BlockLogic prefab = getPrefabForLayer(layer);
        if (prefab == null)
        {
            Debug.LogWarning("[BlockPrefabProcessor] Немає префаба для шару " + layer);
            return null;
        }
        Vector3 position = grid.GetCellCenterWorld(worldPos);

        BlockLogic block = Instantiate(prefab, position, Quaternion.identity, getParentForLayer(layer));
        block.Init(worldPos, layer, state.blockData, state.durability);

        activeBlocks[(worldPos, layer)] = block;
        return block;
    }

    public void RemoveBlockPrefab(Vector3Int worldPos, TileLayer layer)
    {
        if (activeBlocks.TryGetValue((worldPos, layer), out BlockLogic block))
        {
            Destroy(block.gameObject);
            WorldEvents.CallBlockDestroyed(worldPos, layer);
            activeBlocks.Remove((worldPos, layer));
        }
    }

    public void UnloadPrefab(Vector3Int worldPos, TileLayer layer)
    {
        if (activeBlocks.TryGetValue((worldPos, layer), out BlockLogic block))
        {
            Destroy(block.gameObject);
            activeBlocks.Remove((worldPos, layer));
        }
    }

    private BlockLogic getPrefabForLayer(TileLayer layer)
    {
        switch (layer)
        {
            case TileLayer.Solid: return solidPrefab;
            case TileLayer.PassableBack: return passableBackPrefab;
            case TileLayer.PassableFront: return passableFrontPrefab;
            default: return null;  
        }
    }

    public Transform getParentForLayer(TileLayer layer)
    {
        switch (layer)
        {
            case TileLayer.Solid: return solid;
            case TileLayer.PassableFront: return passableFront;
            case TileLayer.PassableBack: return passableBack;
            default: return solid;
        }
    }

    public BlockLogic ActivateTile(Vector3Int position, TileLayer layer) 
    { 
        MiningTile tile = tilemapWriter.GetTile(position, layer);
        if (tile == null) return null;
        if (tile.getBlockData() == null) return null;

        tilemapWriter.ChangeTile(position, layer, null);

        PrefabState state = new PrefabState { blockData = tile.getBlockData(), durability = tile.getBlockData().durability };
        BlockLogic block = spawnBlockPrefab(position, layer, state);

        WorldEvents.CallBlockStateChanged(position, layer, state);
        return block;   
    }
        
}
