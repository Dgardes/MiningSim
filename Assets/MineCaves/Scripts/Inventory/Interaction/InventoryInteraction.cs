using UnityEngine;

//
//
public class InventoryInteraction : MonoBehaviour
{
    public static InventoryInteraction instance;

    [SerializeField] private ItemNameLabel nameLabel;
    [SerializeField] private SplitPopup splitPopup;

    private SlotUI selectedSlot;
    private bool splitMode;

    private void OnEnable()
    {
        instance = this;
        HideAll();
    }

    private void OnDisable()
    {
        ClearSelection();
        if (instance == this) instance = null;
    }

    #region Вхід від слотів

    public void OnSlotTapped(SlotUI slot)
    {
        if (selectedSlot == null)
        {
            TrySelect(slot);
            return;
        }

        if (slot == selectedSlot)
        {
            ClearSelection();
            return;
        }

        if (!IsSelectionValid())
        {
            ClearSelection();
            TrySelect(slot);
            return;
        }

        bool done = splitMode ? TrySplitTo(slot) : TryMoveOrSwap(slot);

        if (done) ClearSelection();
        else slot.PlayDeny();
    }

    public void OnSlotLongPressed(SlotUI slot)
    {
        InventorySlot data = GetData(slot);
        if (data == null || data.IsEmpty()) return;

        if (data.count < 2)
        {
            ClearSelection();
            TrySelect(slot);
            return;
        }

        ClearSelection();
        selectedSlot = slot;
        splitMode = true;
        slot.SetSelected(true);

        if (splitPopup != null)
        {
            int half = Mathf.Max(1, data.count / 2);
            splitPopup.Open(slot.Rect, data.ItemName, data.count, half);
        }
    }

    public void OnSlotDisabled(SlotUI slot)
    {
        if (slot == selectedSlot) ClearSelection();
    }

    #endregion

    #region Вибір

    private void TrySelect(SlotUI slot)
    {
        InventorySlot data = GetData(slot);
        if (data == null || data.IsEmpty()) return;

        selectedSlot = slot;
        splitMode = false;
        slot.SetSelected(true);

        if (nameLabel != null) nameLabel.Show(slot.Rect, data.ItemName);
    }

    public void ClearSelection()
    {
        if (selectedSlot != null) selectedSlot.SetSelected(false);
        selectedSlot = null;
        splitMode = false;
        HideAll();
    }

    private void HideAll()
    {
        if (nameLabel != null) nameLabel.Hide();
        if (splitPopup != null) splitPopup.Close();
    }

    private bool IsSelectionValid()
    {
        if (selectedSlot == null || !selectedSlot.isActiveAndEnabled) return false;
        InventorySlot data = GetData(selectedSlot);
        return data != null && !data.IsEmpty();
    }

    #endregion

    #region Операції з даними

    private bool TryMoveOrSwap(SlotUI target)
    {
        IInventoryLogic fromInv = selectedSlot.Owner;
        IInventoryLogic toInv = target.Owner;
        int fromIndex = selectedSlot.Index;
        int toIndex = target.Index;

        InventorySlot from = fromInv.GetSlot(fromIndex);
        InventorySlot to = GetData(target);
        if (to == null) return false;

        bool sameInventory = fromInv == toInv;

        if (to.IsEmpty() || to.item == from.item)
        {
            if (!toInv.CanPlaceItem(toIndex, from.item)) return false;

            int amount = from.count;
            if (!sameInventory) amount = Mathf.Min(amount, toInv.GetFreeCapacity());
            if (amount <= 0) return false;

            MoveAmount(from, to, amount);
        }
        else
        {
            if (!toInv.CanPlaceItem(toIndex, from.item)) return false;
            if (!fromInv.CanPlaceItem(fromIndex, to.item)) return false;

            if (!sameInventory)
            {
                if (from.count - to.count > toInv.GetFreeCapacity()) return false;
                if (to.count - from.count > fromInv.GetFreeCapacity()) return false;
            }

            ItemData tempItem = to.item;
            int tempCount = to.count;
            to.item = from.item;
            to.count = from.count;
            from.item = tempItem;
            from.count = tempCount;
        }

        NotifyChanged(fromInv, toInv);
        return true;
    }

    private bool TrySplitTo(SlotUI target)
    {
        IInventoryLogic fromInv = selectedSlot.Owner;
        IInventoryLogic toInv = target.Owner;

        InventorySlot from = fromInv.GetSlot(selectedSlot.Index);
        InventorySlot to = GetData(target);
        
        if (to == null) return false;
        bool sameInventory = fromInv == toInv;

        if(!toInv.CanPlaceItem(target.Index, from.item)) return false;
        int amount = splitPopup != null ? splitPopup.Amount : from.count;
        amount = Mathf.Clamp(amount, 1, from.count);

        if (to.IsEmpty() || to.item == from.item)
        { 
            if (!sameInventory) amount = Mathf.Min(amount, toInv.GetFreeCapacity());
            if (amount <= 0) return false;

            MoveAmount(from, to, amount);
            NotifyChanged(fromInv, toInv);
            return true;
        }
        return false;
    }

    private static void MoveAmount(InventorySlot from, InventorySlot to, int amount)
    {
        if (to.IsEmpty()) to.count = 0;

        to.item = from.item;
        to.count += amount;
        from.count -= amount;

        if (from.count <= 0) from.Clear();
    }

    private static void NotifyChanged(IInventoryLogic a, IInventoryLogic b)
    {
        a.OnDataChanged();
        if (b != a) b.OnDataChanged();
    }

    private static InventorySlot GetData(SlotUI slot)
    {
        if (slot == null || slot.Owner == null) return null;
        if (slot.Index < 0 || slot.Index >= slot.Owner.SlotCount) return null;
        return slot.Owner.GetSlot(slot.Index);
    }

    #endregion
}
