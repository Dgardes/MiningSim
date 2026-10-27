using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class WorldHash
{
    public static float Hash01(int seed, int x, int y, int salt)
    {
        unchecked
        {
            uint h = (uint)seed;
            h ^= (uint)x * 0x27d4eb2dU;
            h ^= (uint)y * 0x165667b1U;
            h ^= (uint)salt * 0x9e3779b9U;
            h = (h ^ (h >> 15)) * 0x85ebca6bU;
            h = (h ^ (h >> 13)) * 0xc2b2ae35U;
            h ^= h >> 16;
            return (h >> 8) / 16777216f;
        }
    }
}

public struct GenerationCell
{
    public MiningTile background;
    public MiningTile solid;
    public MiningTile passableFront;
    public MiningTile passableBack;
    public MiningTile liquid;
}

[Serializable]
public class Depthlayer
{
    public int minDepth;
    public int maxDepth;
    public MiningTile filler;
    public MiningTile background;
    public bool blend = true;
}

public class ChunkGenerator : MonoBehaviour
{

    [Header("Посилання")]
    [SerializeField] private WorldGenSettingsSO worldGenSettings;
    [SerializeField] private TilemapWriter tilemapWriter;

    [Header("Генерація")]
    [SerializeField] private int seed = 12345;
    [SerializeField] private int borderSize = 1;

    private int chunkSize = ChunkCord.chunkSize;
    private GenerationCell[,] generationCells;

    private Vector2 caveOffset;
    private CaveSettings c => worldGenSettings.Caves;

    private readonly List<int> familyOrder = new List<int>();
    private readonly List<OreCandidate> oreCandidates = new List<OreCandidate>();

    private static readonly Comparison<OreCandidate> byDistance = (p, q) => p.d.CompareTo(q.d);

    private struct OreCandidate
    {
        public int x;
        public int y;
        public float d;
    }

    public int Seed => seed;

    private void Start()
    {
        generationCells = new GenerationCell[chunkSize + 2 * borderSize, chunkSize + 2 * borderSize];
        caveOffset = new Vector2(seed + 5000, seed + 5000);
    }

    public void SetSeed(int newSeed)
    {
        seed = newSeed;
        caveOffset = new Vector2(seed + 5000, seed + 5000);
    }

    public void GenerateChunk(Chunk chunkOverrides)
    {
        Array.Clear(generationCells, 0, generationCells.Length);

        generateBase(chunkOverrides.chunkCord);
        generateOres(chunkOverrides.chunkCord);
        applyOverridesToChunk(chunkOverrides);
        writeChunk(chunkOverrides.chunkCord);
    }

    #region Base (Layers and Caves)

    private void generateBase(ChunkCord chunkCord)
    {
        for (int x = 0; x < chunkSize + 2 * borderSize; x++)
        {
            for (int y = 0; y < chunkSize + 2 * borderSize; y++)
            {

                Vector3Int worldPos = chunkCord.LocalToWorld(x - borderSize, y - borderSize);
                if(worldPos.x < c.startGenerationCordX) continue;

                int depth = -worldPos.y;

                float caveNoise = getCaveNoise(worldPos.x, worldPos.y);

                float tY = Mathf.InverseLerp(c.safeDepth, c.safeDepth + c.fadeDepth, depth);
                float thresholdY = Mathf.Lerp(1f, c.threshold, tY);

                float tX = Mathf.InverseLerp(c.startGenerationCordX, c.startGenerationCordX + c.fadeWidth, worldPos.x);
                float thresholdX = Mathf.Lerp(1f, c.threshold, tX);

                float threshold = Mathf.Max(thresholdX, thresholdY);

                Depthlayer realLayer = getLayerForDepth(depth);
                if (realLayer == null) continue;
                Depthlayer layer = realLayer;

                if (realLayer.blend)
                {
                    int jitterDepth = Mathf.RoundToInt(WorldHash.Hash01(seed, worldPos.x, worldPos.y, 0) * c.blend);
                    int layerDepth = WorldHash.Hash01(seed, worldPos.x, worldPos.y, 1) < 0.5f ?
                        depth + jitterDepth : depth - jitterDepth;
                    Depthlayer jitteredLayer = getLayerForDepth(layerDepth);

                    if (jitteredLayer != null && jitteredLayer.blend)
                        layer = jitteredLayer;
                }

                MiningTile tile = caveNoise < threshold ? layer.filler : null;

                GenerationCell cell = new GenerationCell();
                cell.solid = tile;
                cell.background = layer.background;
                generationCells[x, y] = cell;
            }
        }
    }

    private Depthlayer getLayerForDepth(int depth)
    {
        foreach (var layer in worldGenSettings.DepthLayers)
        {
            if (depth >= layer.minDepth && depth <= layer.maxDepth)
            {
                return layer;
            }
        }
        return null;
    }

    private float getCaveNoise(int worldX, int worldY)
    {
        float x = (worldX + caveOffset.x) * c.scale * c.stretchX;
        float y = (worldY + caveOffset.y) * c.scale * c.stretchY;

        float value = 0.0f;
        float noiseWeight = 0.0f;
        int octaves = c.octaves;
        float frequency = 1f;
        float amplitude = 1f;

        for (int i = 0; i < octaves; i++)
        {
            value += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            noiseWeight += amplitude;

            frequency *= c.frequencyMultiplier;
            amplitude *= c.amplitudeMultiplier;
        }

        return value / noiseWeight;
    }

    #endregion

    #region Ores

    private void generateOres(ChunkCord chunkCord)
    {
        int bufferSize = chunkSize + 2 * borderSize;

        Vector3Int bufferMin = chunkCord.LocalToWorld(-borderSize, -borderSize);

        int bufferMaxX = bufferMin.x + bufferSize - 1;
        int bufferMaxY = bufferMin.y + bufferSize - 1;

        List<OreFamilySO> families = worldGenSettings.OreFamilies;

        familyOrder.Clear();
        for (int i = 0; i < families.Count; i++)
        {
            if (families[i] != null) familyOrder.Add(i);
        }
        familyOrder.Sort(compareFamilies);

        foreach (int f in familyOrder)
        {
            OreFamilySO family = families[f];

            int oreChunkSize = family.oreChunkSize;
            int reach = getMaxReach(family);

            int saltBase = 100 + f * 20;

            int oreChunkMinX = Mathf.FloorToInt((bufferMin.x - reach) / (float)oreChunkSize);
            int oreChunkMaxX = Mathf.FloorToInt((bufferMaxX + reach) / (float)oreChunkSize);
            int oreChunkMinY = Mathf.FloorToInt((bufferMin.y - reach) / (float)oreChunkSize);
            int oreChunkMaxY = Mathf.FloorToInt((bufferMaxY + reach) / (float)oreChunkSize);

            for (int oreX = oreChunkMinX; oreX <= oreChunkMaxX; oreX++)
            {
                for (int oreY = oreChunkMinY; oreY <= oreChunkMaxY; oreY++)
                {
                    generateVein(family, oreX, oreY, saltBase, bufferMin, bufferSize);
                }
            }
        }
    }

    private int compareFamilies(int p, int q)
    {
        List<OreFamilySO> families = worldGenSettings.OreFamilies;
        int byPriority = families[p].priority.CompareTo(families[q].priority);
        if (byPriority != 0) return byPriority;
        return p.CompareTo(q);
    }

    private int getMaxReach(OreFamilySO family)
    {
        int biggestVein = 0;
        foreach (OreTier tier in family.tiers)
        {
            if (tier != null) biggestVein = Mathf.Max(biggestVein, tier.maxVeinSize);
        }
        return getVeinRadius(biggestVein, family.maxElongation);
    }

    private int getVeinRadius(int veinSize, float elongation)
    {
        float a = Mathf.Sqrt(veinSize * elongation / Mathf.PI);
        return Mathf.CeilToInt(a) + 1;
    }

    private void generateVein(OreFamilySO family, int oreX, int oreY, int saltBase, Vector3Int bufferMin, int bufferSize)
    {
        int oreChunkSize = family.oreChunkSize;
        float margin = family.oreOffset * oreChunkSize;

        float zoneMinX = oreX * oreChunkSize + margin;
        float zoneMaxX = (oreX + 1) * oreChunkSize - margin;
        float zoneMinY = oreY * oreChunkSize + margin;
        float zoneMaxY = (oreY + 1) * oreChunkSize - margin;

        float randomX = WorldHash.Hash01(seed, oreX, oreY, saltBase + 0);
        float randomY = WorldHash.Hash01(seed, oreX, oreY, saltBase + 1);

        float centerX = Mathf.Lerp(zoneMinX, zoneMaxX, randomX);
        float centerY = Mathf.Lerp(zoneMinY, zoneMaxY, randomY);

        int depth = -Mathf.FloorToInt(centerY);
        OreTier tier = family.GetTierForDepth(depth);
        if (tier == null || tier.blockData == null) return;

        float chance = tier.EvaluateSpawnChance(depth);
        if (WorldHash.Hash01(seed, oreX, oreY, saltBase + 2) >= chance) return;

        MiningTile oreTile = worldGenSettings.BlockDatabase.GetTile(tier.blockData.code);
        if (oreTile == null) return;

        int sizeVariants = tier.maxVeinSize - tier.minVeinSize + 1;
        int veinSize = tier.minVeinSize + Mathf.FloorToInt(WorldHash.Hash01(seed, oreX, oreY, saltBase + 3) * sizeVariants);
        if (veinSize <= 0) return;

        float elongation = Mathf.Lerp(1f, family.maxElongation, WorldHash.Hash01(seed, oreX, oreY, saltBase + 4));
        float angle = WorldHash.Hash01(seed, oreX, oreY, saltBase + 5) * Mathf.PI;

        float a = Mathf.Sqrt(veinSize * elongation / Mathf.PI);
        float b = Mathf.Sqrt(veinSize / (elongation * Mathf.PI));

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        int r = getVeinRadius(veinSize, elongation);
        int centerBlockX = Mathf.FloorToInt(centerX);
        int centerBlockY = Mathf.FloorToInt(centerY);

        oreCandidates.Clear();
        for (int blockX = centerBlockX - r; blockX <= centerBlockX + r; blockX++)
        {
            for (int blockY = centerBlockY - r; blockY <= centerBlockY + r; blockY++)
            {
                float dx = (blockX + 0.5f) - centerX;
                float dy = (blockY + 0.5f) - centerY;

                float u = dx * cos + dy * sin;
                float v = -dx * sin + dy * cos;

                float d = Mathf.Sqrt((u / a) * (u / a) + (v / b) * (v / b));

                OreCandidate candidate = new OreCandidate();
                candidate.x = blockX;
                candidate.y = blockY;
                candidate.d = d;
                oreCandidates.Add(candidate);
            }
        }

        oreCandidates.Sort(byDistance);
        int count = Mathf.Min(veinSize, oreCandidates.Count);

        for (int i = 0; i < count; i++)
        {
            OreCandidate candidate = oreCandidates[i];

            int arrayX = candidate.x - bufferMin.x;
            int arrayY = candidate.y - bufferMin.y;

            if (arrayX < 0 || arrayY < 0 || arrayX >= bufferSize || arrayY >= bufferSize) continue;

            GenerationCell cell = generationCells[arrayX, arrayY];
            if (cell.solid == null) continue;
            cell.solid = oreTile;

            generationCells[arrayX, arrayY] = cell;
        }
    }

    #endregion

    #region Player overrides

    private void applyOverridesToChunk(Chunk chunkOverrides)
    {
        foreach (var kvp in chunkOverrides.Overrides)
        {
            int arrayX = kvp.Key.x + borderSize;
            int arrayY = kvp.Key.y + borderSize;
            GenerationCell cell = generationCells[arrayX, arrayY];

            cell.solid = kvp.Value.solid;
            cell.passableFront = kvp.Value.passableFront;
            cell.passableBack = kvp.Value.passableBack;
            cell.liquid = kvp.Value.liquid;

            generationCells[arrayX, arrayY] = cell;
        }
    }

    #endregion

    #region Tilemap write

    private void writeChunk(ChunkCord chunkCord)
    {
        for (int x = borderSize; x < chunkSize + borderSize; x++)
        {
            for (int y = borderSize; y < chunkSize + borderSize; y++)
            {
                GenerationCell cell = generationCells[x, y];
                Vector3Int cellPosition = chunkCord.LocalToWorld(x - borderSize, y - borderSize);
                if (cell.background != null)
                    tilemapWriter.PlaceTile(cellPosition, TileLayer.Background, cell.background);
                if (cell.solid != null)
                    tilemapWriter.PlaceTile(cellPosition, TileLayer.Solid, cell.solid);
                if (cell.passableBack != null)
                    tilemapWriter.PlaceTile(cellPosition, TileLayer.PassableBack, cell.passableBack);
                if (cell.passableFront != null)
                    tilemapWriter.PlaceTile(cellPosition, TileLayer.PassableFront, cell.passableFront);
                if (cell.liquid != null)
                    tilemapWriter.PlaceTile(cellPosition, TileLayer.Liquid, cell.liquid);
            }
        }
    }

    #endregion
}
