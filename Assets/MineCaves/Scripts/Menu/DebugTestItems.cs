using UnityEngine;

public static class DebugTestItems
{
    public static int GiveAll(IInventoryLogic inventory, int minAmount, int maxAmount)
    {
        if (inventory == null) return 0;

        ItemData[] items = Resources.LoadAll<ItemData>("Items");
        int given = 0;
        foreach (ItemData item in items)
        {
            int amount = Random.Range(minAmount, maxAmount + 1);
            given += inventory.TryReceiveItem(item, amount);
        }

        inventory.OnDataChanged();
        Debug.Log("[DebugTestItems] Видано предметів: " + given + " (видів: " + items.Length + ")");
        return given;
    }

    public static void Clear(IInventoryLogic inventory)
    {
        if (inventory == null) return;
        for (int i = 0; i < inventory.SlotCount; i++) inventory.GetSlot(i).Clear();
        inventory.OnDataChanged();
    }

    public static bool IsEmpty(IInventoryLogic inventory)
    {
        if (inventory == null) return true;
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            if (!inventory.GetSlot(i).IsEmpty()) return false;
        }
        return true;
    }
}
