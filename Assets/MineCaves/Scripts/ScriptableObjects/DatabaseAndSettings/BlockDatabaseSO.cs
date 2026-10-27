using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDatabase", menuName = "Scriptable Objects/BlockDatabaseSO")]
public class BlockDatabaseSO : ScriptableObject
{
    public static BlockDatabaseSO instance;

    [SerializeField] private List<MiningTile> allTiles;

    private Dictionary<string, MiningTile> byCode;

    private void OnEnable()
    {
        instance = this;
        BuildIndex();
    }

    private void BuildIndex()
    {
        byCode = new Dictionary<string, MiningTile>();
        foreach (var tile in allTiles)
        {
            if (tile == null || tile.getBlockData() == null) continue;
            string code = tile.getBlockData().code;
            if (string.IsNullOrEmpty(code)) continue;

            if (byCode.ContainsKey(code))
                Debug.LogWarning("[BlockDatabaseSO] Дубльований code: " + code + " на тайлі " + tile.name);
            else
                byCode[code] = tile;
        }
    }

    public MiningTile GetTile(string code)
    {
        if (byCode == null) BuildIndex();
        if (string.IsNullOrEmpty(code)) return null;
        byCode.TryGetValue(code, out var tile);
        return tile;
    }

    public BlockData GetBlockData(string code)
    {
        var tile = GetTile(code);
        return tile?.getBlockData();
    }
}
