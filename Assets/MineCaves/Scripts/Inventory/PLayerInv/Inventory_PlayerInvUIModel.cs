using UnityEngine;
using System.Collections.Generic;

public class Inventory_PlayerInvUIModel : MonoBehaviour, IInventoryUIModule
{
    [SerializeField] private Transform contentParent; 
    [SerializeField] private SlotUI slotPrefab;        

    private List<SlotUI> spawnedSlots = new List<SlotUI>();

    private void OnEnable()
    {
        BuildGridButtons();
        RefreshModuleUI();
    }

    private void BuildGridButtons()
    {
        if (contentParent == null || slotPrefab == null) return;

        foreach (Transform child in contentParent) Destroy(child.gameObject);
        spawnedSlots.Clear();

        int size = Inventory_PlayerInvLogic.instance != null ? Inventory_PlayerInvLogic.instance.SlotCount : 20;

        for (int i = 0; i < size; i++)
        {
            SlotUI newSlot = Instantiate(slotPrefab, contentParent);
            newSlot.InitSlot(i, Inventory_PlayerInvLogic.instance);
            spawnedSlots.Add(newSlot);
        }
    }

    public void RefreshModuleUI()
    {
        if (spawnedSlots == null || spawnedSlots.Count == 0) return;
        if (Inventory_PlayerInvLogic.instance == null) return;

        List<InventorySlot> backpackData = Inventory_PlayerInvLogic.instance.GetInventoryList();

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            if (i < backpackData.Count && spawnedSlots[i] != null)
            {
                spawnedSlots[i].UpdateSlotVisual(backpackData[i]);
            }
        }
    }
}
