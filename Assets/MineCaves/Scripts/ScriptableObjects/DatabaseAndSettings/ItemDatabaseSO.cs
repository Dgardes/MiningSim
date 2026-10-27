using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    [SerializeField] private List<ItemData> allItems = new List<ItemData>();

    private Dictionary<string, ItemData> byCode;

    private void OnEnable()
    {
        byCode = null;
    }

    private void BuildIndex()
    {
        byCode = new Dictionary<string, ItemData>();
        foreach (ItemData item in allItems)
        {
            if (item == null || string.IsNullOrEmpty(item.code)) continue;

            if (byCode.ContainsKey(item.code))
                Debug.LogWarning("[ItemDatabaseSO] Дубльований code: " + item.code + " у " + item.name);
            else
                byCode[item.code] = item;
        }
    }

    public ItemData GetItem(string code)
    {
        if (string.IsNullOrEmpty(code)) return null;
        if (byCode == null) BuildIndex();
        byCode.TryGetValue(code, out ItemData item);
        return item;
    }

    public IReadOnlyList<ItemData> AllItems => allItems;

#if UNITY_EDITOR
    [ContextMenu("Collect all items")]
    private void CollectAllItems()
    {
        allItems.Clear();
        foreach (string guid in AssetDatabase.FindAssets("t:ItemData"))
        {
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid));
            if (item == null) continue;

            if (string.IsNullOrEmpty(item.code))
            {
                item.code = item.name;
                EditorUtility.SetDirty(item);
            }
            allItems.Add(item);
        }

        allItems.Sort((a, b) => string.CompareOrdinal(a.code, b.code));
        byCode = null;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log("[ItemDatabaseSO] Зібрано предметів: " + allItems.Count);
    }
#endif
}
