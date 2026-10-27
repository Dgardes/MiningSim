using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Inventory_PlayerInvLogic : MonoBehaviour, IInventoryLogic
{
    public static Inventory_PlayerInvLogic instance;

    [Header("Local Backpack Data")]
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private int currentMaxCapacity = 100;
    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();
    [SerializeField] private ItemDatabaseSO itemDatabase;

    private List<SlotUI> activeUISlots = new List<SlotUI>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        LoadLocalInventory();
    }

    public void UpdateMaxCapacity(int newLimit)
    {
        currentMaxCapacity = newLimit;
        Inventory_Manager.instance.TriggerUIUpdate();
    }

    #region IInventoryLogic implementation
    public InventorySlot GetSlot(int index) => inventory[index];

    public int SlotCount => inventory.Count;

    public int GetFreeCapacity() => Mathf.Max(0, currentMaxCapacity - GetTotalItemCount());

    public bool CanPlaceItem(int index, ItemData item) => true;

    public int TryReceiveItem(ItemData item, int amount)
    {
        int currentTotal = GetTotalItemCount();
        int freeSpace = currentMaxCapacity - currentTotal;

        if (freeSpace <= 0) return 0;

        int amountToTake = Mathf.Min(amount, freeSpace);

        
        int firstEmpty = -1;
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i].IsEmpty() && firstEmpty == -1) firstEmpty = i;

            if (inventory[i].item == item)
            {
                inventory[i].count += amountToTake;
                return amountToTake;
            }
        }

        if (firstEmpty != -1)
        {
            inventory[firstEmpty].item = item;
            inventory[firstEmpty].count = amountToTake;
            return amountToTake;
        }

        return 0;
    }

    public void OnDataChanged()
    {
        SaveLocalInventory();
        if (Inventory_Manager.instance != null)
        {
            Inventory_Manager.instance.TriggerUIUpdate();
        }
    }


    #endregion

    #region Helper methods 
    public List<InventorySlot> GetInventoryList() => inventory;

    private int GetTotalItemCount()
    {
        int total = 0;
        foreach (var slot in inventory) if (!slot.IsEmpty()) total += slot.count;
        return total;
    }

    #endregion

    private void SaveLocalInventory()
    {
        List<InventorySlotSaveData> saveData = new List<InventorySlotSaveData>();

        foreach (var slot in inventory)
        {
            saveData.Add(new InventorySlotSaveData
            {
                itemCode = (slot.item != null) ? slot.item.code : "Empty",
                count = slot.count
            });
        }

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        DataManager.SaveInventoryData(json);
    }

    private void LoadLocalInventory()
    {
        string json = DataManager.LoadInventoryData();
        if (itemDatabase == null) Debug.LogWarning("[Inventory_PlayerInvLogic] Не призначено ItemDatabase");
        inventory.Clear();
        
        if (string.IsNullOrEmpty(json))
        {
            for (int i = 0; i < inventorySize; i++) inventory.Add(new InventorySlot(null, 0));
            return;
        }

        List<InventorySlotSaveData> loaded = JsonConvert.DeserializeObject<List<InventorySlotSaveData>>(json);
        
        for (int i = 0; i < inventorySize; i++)
        {
            if (i < loaded.Count && loaded[i].itemCode != "Empty")
            {
                ItemData asset = itemDatabase != null ? itemDatabase.GetItem(loaded[i].itemCode) : null;
                
                if (asset != null)
                {
                    inventory.Add(new InventorySlot(asset, loaded[i].count));
                }
                else
                {
                    inventory.Add(new InventorySlot(null, 0));
                }
            }
            else 
            {
                inventory.Add(new InventorySlot(null, 0));
            }
        }
    }
}

[System.Serializable]
public struct InventorySlotSaveData
{
    public string itemCode;
    public int count;
}