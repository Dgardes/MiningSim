using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenSettings", menuName = "Scriptable Objects/WorldGenSettingsSO")]
public class WorldGenSettingsSO :ScriptableObject
{
    [SerializeField] private BlockDatabaseSO blockDatabase;
    [SerializeField] private List<OreFamilySO> oreFamilies;
    [SerializeField] private List<Depthlayer> depthLayers;
    [SerializeField] private CaveSettings caves = new CaveSettings();

    public BlockDatabaseSO BlockDatabase => blockDatabase;
    public List<OreFamilySO> OreFamilies => oreFamilies;
    public List<Depthlayer> DepthLayers => depthLayers;
    public CaveSettings Caves => caves;

    
    public event Action Changed;

    private void OnValidate()
    {
        if(Application.isPlaying) Changed?.Invoke();
    }
}

[Serializable]
public class CaveSettings
{
    [Range(0.0f, 1f)] public float scale = 0.06f;
    [Range(1, 6)] public int octaves = 3;
    [Range(0.0f, 1f)] public float threshold = 0.62f;

    [Range(1f, 4f)] public float frequencyMultiplier = 2f;
    [Range(0.1f, 1f)] public float amplitudeMultiplier = 0.5f;

    [Range(0.1f, 4f)] public float stretchX = 1f;
    [Range(0.1f, 4f)] public float stretchY = 1f;
    
    [Range(0, 100)] public int safeDepth = 50;
    public int startGenerationCordX = 0;

    [Range(0, 50)] public int fadeDepth = 10;
    [Range(0, 50)] public int fadeWidth = 10;


    [Range(0, 50)] public int blend = 10;
    // public AnimationCurve thresholdByDepth = AnimationCurve.Constant(0, 1, 1);
}
