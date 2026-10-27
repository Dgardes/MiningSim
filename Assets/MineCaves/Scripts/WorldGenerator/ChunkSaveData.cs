using System.Collections.Generic;

[System.Serializable]
public struct CellOverrideRecord
{
    public int x, y;
    public string solidCode;
    public string backCode;
    public string frontCode;
    public string liquidCode;
}

[System.Serializable]
public struct PrefabsRecord
{
    public int x, y;
    public int layer;          
    public string prefabCode;  
    public int durability;
}

[System.Serializable]
public class ChunkSaveData
{
    public int chunkX, chunkY;
    public List<CellOverrideRecord> cells = new List<CellOverrideRecord>();
    public List<PrefabsRecord> prefabs = new List<PrefabsRecord>();
}
