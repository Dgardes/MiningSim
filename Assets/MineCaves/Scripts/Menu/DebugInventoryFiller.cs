using UnityEngine;

public class DebugInventoryFiller : MonoBehaviour
{
    [SerializeField] private bool fillIfEmptyOnStart = true;
    [SerializeField] private int minAmount = 5;
    [SerializeField] private int maxAmount = 15;

    private void Start()
    {
        if (!fillIfEmptyOnStart) return;

        Inventory_PlayerInvLogic inventory = Inventory_PlayerInvLogic.instance;
        if (DebugTestItems.IsEmpty(inventory)) DebugTestItems.GiveAll(inventory, minAmount, maxAmount);
    }

    [ContextMenu("Give test items")]
    private void GiveNow()
    {
        DebugTestItems.GiveAll(Inventory_PlayerInvLogic.instance, minAmount, maxAmount);
    }

    [ContextMenu("Clear inventory")]
    private void ClearNow()
    {
        DebugTestItems.Clear(Inventory_PlayerInvLogic.instance);
    }
}
