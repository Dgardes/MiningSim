using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;
    public string ItemName => item != null ? item.itemName : "Empty";

    public InventorySlot(ItemData newItem, int newCount)
    {
        item = newItem;
        count = newCount;
    }

    public bool IsEmpty()
    {
        return item == null || count <= 0;
    }

    public void Clear()
    {
        item = null;
        count = 0;
    }
}