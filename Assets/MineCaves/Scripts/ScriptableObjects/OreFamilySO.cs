using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "OreFamily", menuName = "Scriptable Objects/OreFamilySO")]
public class OreFamilySO : ScriptableObject
{
    public string familyName;

    [Header("Generation")]
    public int priority = 0;

    [Range(4, 64)] public int oreChunkSize = 16;
    [Range(0f, 0.45f)] public float oreOffset = 0.2f;
    [Range(1f, 4f)] public float maxElongation = 2f;


    public List<OreTier> tiers = new List<OreTier>();

    public OreTier GetTierForDepth(int depth)
    {
        foreach (var tier in tiers)
        {
            if (depth >= tier.minDepth && depth <= tier.maxDepth) return tier;
        }
        return null;
    }
}

[Serializable]
public class OreTier
{
    public BlockData blockData;

    [Header("Depth range")]
    [FormerlySerializedAs("minHeight")] public int minDepth;
    [FormerlySerializedAs("maxHeight")] public int maxDepth;

    [Header("Spawn chance")]
    public AnimationCurve spawnChanceByDepth = AnimationCurve.Linear(0, 1, 1, 1);

    [Header("Vein size")]
    [Min(1)] public int minVeinSize = 1;
    [Min(1)] public int maxVeinSize = 5;

    public float EvaluateSpawnChance(int depth)
    {
        if (spawnChanceByDepth == null || spawnChanceByDepth.length == 0) return 1f;

        float t = Mathf.InverseLerp(minDepth, maxDepth, depth);
        return spawnChanceByDepth.Evaluate(t);
    }
}