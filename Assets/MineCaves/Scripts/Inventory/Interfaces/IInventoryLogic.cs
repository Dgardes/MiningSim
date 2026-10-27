public interface IInventoryLogic
{
    int SlotCount { get; }
    InventorySlot GetSlot(int index);
    int TryReceiveItem(ItemData item, int amount);
    int GetFreeCapacity();
    bool CanPlaceItem(int index, ItemData item);
    void OnDataChanged();
}
