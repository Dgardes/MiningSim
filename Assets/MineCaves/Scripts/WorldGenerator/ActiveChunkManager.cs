using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActiveChunkManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ChunkGenerator chunkGenerator;
    
    [SerializeField] private TilemapWriter tilemapWriter;
    [SerializeField] private BlockPrefabProcessor blockPrefabProcessor;
    

    private ChunkCord currentPlayerChunk;

    [SerializeField] private int loadRadius = 2;

    private Dictionary<ChunkCord, Chunk> loadedChunks = new Dictionary<ChunkCord, Chunk>();

    public IEnumerable<Chunk> LoadedChunks => loadedChunks.Values;

    private void OnEnable()
    {
        WorldEvents.TileChanged += OnTileChanged;
        WorldEvents.BlockStateChanged += OnBlockStateChanged;
        WorldEvents.BlockDestroyed += OnBlockDestroyed;
    }

    private void OnDisable()
    {
        WorldEvents.TileChanged -= OnTileChanged;
        WorldEvents.BlockStateChanged -= OnBlockStateChanged;
        WorldEvents.BlockDestroyed -= OnBlockDestroyed;
    }

    void Update()
    {
        ChunkCord playerchunk = ChunkCord.worldToCell(playerTransform.position);
        if (currentPlayerChunk == playerchunk)
        {
            return;
        }
        currentPlayerChunk = playerchunk;

        List<Chunk> newActiveChunks = new List<Chunk>();
        List< Chunk> chunksToLoad = new List<Chunk>();
        List<Chunk> chunksToUnload = new List<Chunk>();

        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                ChunkCord chunkCord = new ChunkCord(playerchunk.x + x, playerchunk.y + y);
                if (loadedChunks.ContainsKey(chunkCord))
                {
                    newActiveChunks.Add(loadedChunks[chunkCord]);
                }
                else
                {  
                    chunksToLoad.Add(getChunkData(chunkCord));
                }
            }
        }
        chunksToUnload = loadedChunks.Values.ToList().FindAll(chunk => !newActiveChunks.Exists(newChunk => newChunk.chunkCord == chunk.chunkCord));
        
        foreach (Chunk chunk in chunksToLoad)
        {
            loadedChunks.Add(chunk.chunkCord, chunk);
            chunkGenerator.GenerateChunk(chunk);
            foreach (var kvp in chunk.PrefabStates)
            {
                Vector3Int worldPos = chunk.chunkCord.LocalToWorld(kvp.Key.x, kvp.Key.y);
                blockPrefabProcessor.spawnBlockPrefab(worldPos, (TileLayer)kvp.Key.z, kvp.Value);
            }
        }

        foreach (Chunk chunk in chunksToUnload)
        {
            unloadChunk(chunk);
            loadedChunks.Remove(chunk.chunkCord);
            
        }
    }

    private Chunk getChunkData(ChunkCord chunkCord)
    {
        return ChunkSaveIO.Load(chunkCord) ?? new Chunk { chunkCord = chunkCord }; ;
    }

    private void OnTileChanged(Vector3Int worldPos, TileLayer layer, MiningTile tile)
    {
        Chunk chunk = getChunk(worldPos);
        if (chunk == null) return;

        Vector2Int localPos = toLocal(worldPos);

        CellContent content = new CellContent 
        { 
            solid = tilemapWriter.GetTile(worldPos, TileLayer.Solid),
            passableBack = tilemapWriter.GetTile(worldPos, TileLayer.PassableBack),
            passableFront = tilemapWriter.GetTile(worldPos, TileLayer.PassableFront),
            liquid = tilemapWriter.GetTile(worldPos, TileLayer.Liquid)
        };

        chunk.SetCell(localPos, content);

    }

    private void OnBlockStateChanged(Vector3Int worldPos, TileLayer layer, PrefabState state)
    {
        Chunk chunk = getChunk(worldPos);
        if (chunk == null) return;

        Vector2Int localPos = toLocal(worldPos);
        Vector3Int key = Chunk.MakePrefabKey(localPos, layer);

        chunk.SetPrefabState(key, state);
    }

    private void OnBlockDestroyed(Vector3Int worldPos, TileLayer layer)
    {
        Chunk chunk = getChunk(worldPos);
        if(chunk == null) return;

        Vector2Int localPos = toLocal(worldPos);

        Vector3Int key = Chunk.MakePrefabKey(localPos, layer);

        chunk.RemovePrefabState(key);
    }

    private void unloadChunk(Chunk chunk)
    { 
        ChunkSaveIO.Save(chunk);

        for (int x = 0; x < ChunkCord.chunkSize; x++)
        {
            for (int y = 0; y < ChunkCord.chunkSize; y++)
            {
                tilemapWriter.PlaceTile(chunk.chunkCord.LocalToWorld(x, y), TileLayer.Background, null);
                tilemapWriter.PlaceTile(chunk.chunkCord.LocalToWorld(x, y), TileLayer.Solid, null);
                tilemapWriter.PlaceTile(chunk.chunkCord.LocalToWorld(x, y), TileLayer.PassableBack, null);
                tilemapWriter.PlaceTile(chunk.chunkCord.LocalToWorld(x, y), TileLayer.PassableFront, null);
                tilemapWriter.PlaceTile(chunk.chunkCord.LocalToWorld(x, y), TileLayer.Liquid, null);

                blockPrefabProcessor.UnloadPrefab(chunk.chunkCord.LocalToWorld(x, y), TileLayer.Solid);
                blockPrefabProcessor.UnloadPrefab(chunk.chunkCord.LocalToWorld(x, y), TileLayer.PassableFront);
                blockPrefabProcessor.UnloadPrefab(chunk.chunkCord.LocalToWorld(x, y), TileLayer.PassableBack);
            }
        }
    }
    
    public void unloadAll()
    {
        foreach (Chunk chunk in loadedChunks.Values)
        {
            unloadChunk(chunk);
        }
        loadedChunks.Clear();
        currentPlayerChunk = null;
    }

    private Chunk getChunk(Vector3Int worldPos)
    {
        ChunkCord cord = ChunkCord.worldToCell(worldPos);
        if(loadedChunks.ContainsKey(cord)) return loadedChunks[cord];
        return null;   
    }

    private Vector2Int toLocal(Vector3Int worldPos)
    {
        ChunkCord cord = ChunkCord.worldToCell(worldPos);
        return new Vector2Int(worldPos.x - cord.x * ChunkCord.chunkSize, worldPos.y - cord.y * ChunkCord.chunkSize);
    }
}
