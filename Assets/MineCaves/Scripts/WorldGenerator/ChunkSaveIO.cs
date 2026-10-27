using System.IO;
using UnityEngine;

public static class ChunkSaveIO
{
    private static readonly string ChunksFolderPath = Path.Combine(Application.persistentDataPath, "MineCaves Data", "Chunks");

    private static string GetPath(ChunkCord coord) => Path.Combine(ChunksFolderPath, coord.x.ToString() + "_" + coord.y.ToString() + ".json");

    public static void Save(Chunk chunk)
    {
        if (!chunk.wasChanged) return;
        try
        {
            if (!Directory.Exists(ChunksFolderPath)) Directory.CreateDirectory(ChunksFolderPath);

            var data = new ChunkSaveData
            {
                chunkX = chunk.chunkCord.x,
                chunkY = chunk.chunkCord.y
            };

            foreach (var kvp in chunk.Overrides)
            {
                var pos = kvp.Key;
                var content = kvp.Value;
                data.cells.Add(new CellOverrideRecord
                {
                    x = pos.x, y = pos.y,
                    solidCode  = content.solid?.getBlockData()?.code  ?? "",
                    backCode   = content.passableBack?.getBlockData()?.code  ?? "",
                    frontCode  = content.passableFront?.getBlockData()?.code ?? "",
                    liquidCode = content.liquid?.getBlockData()?.code ?? "",
                });
            }

            foreach (var kvp in chunk.PrefabStates)
            {
                Vector3Int key = kvp.Key; 
                PrefabState state = kvp.Value;
                if (state.blockData == null) continue;

                data.prefabs.Add(new PrefabsRecord
                {
                    x = key.x, y = key.y,
                    layer = key.z,
                    prefabCode = state.blockData.code,
                    durability = state.durability
                });
            }

            File.WriteAllText(GetPath(chunk.chunkCord), JsonUtility.ToJson(data));
            chunk.wasChanged = false;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ChunkSaveIO] Помилка збереження чанка " + chunk.chunkCord.x + "," + chunk.chunkCord.y + ": " + e.Message);
        }
    }

    public static Chunk Load(ChunkCord coord)
    {
        string path = GetPath(coord);
        if (!File.Exists(path)) return null;

        ChunkSaveData data;
        try { data = JsonUtility.FromJson<ChunkSaveData>(File.ReadAllText(path)); }
        catch (System.Exception e)
        {
            Debug.LogError("[ChunkSaveIO] Помилка завантаження чанка " + coord.x + "," + coord.y + ": " + e.Message);
            return null;
        }

        if (data == null) return null;

        var chunk = new Chunk { chunkCord = coord };
        ApplyToChunk(chunk, data);
        chunk.wasChanged = false;
        return chunk;
    }

    private static void ApplyToChunk(Chunk chunk, ChunkSaveData data)
    {
        foreach (var cell in data.cells)
        {
            var content = new CellContent
            {
                solid         = string.IsNullOrEmpty(cell.solidCode)  ? null : BlockDatabaseSO.instance.GetTile(cell.solidCode),
                passableBack  = string.IsNullOrEmpty(cell.backCode)   ? null : BlockDatabaseSO.instance.GetTile(cell.backCode),
                passableFront = string.IsNullOrEmpty(cell.frontCode)  ? null : BlockDatabaseSO.instance.GetTile(cell.frontCode),
                liquid        = string.IsNullOrEmpty(cell.liquidCode) ? null : BlockDatabaseSO.instance.GetTile(cell.liquidCode),
            };
            chunk.SetCell(new Vector2Int(cell.x, cell.y), content);
        }

        foreach (var record in data.prefabs)
        {
            BlockData blockData = BlockDatabaseSO.instance.GetBlockData(record.prefabCode);
            if (blockData == null)
            {
                Debug.LogWarning("[ChunkSaveIO] Невідомий код блоку в збереженні: " + record.prefabCode);
                continue;
            }

            PrefabState state = new PrefabState();
            state.blockData = blockData;
            state.durability = record.durability;

            chunk.SetPrefabState(new Vector3Int(record.x, record.y, record.layer), state);
        }
    }
}
