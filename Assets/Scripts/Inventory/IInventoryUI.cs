using UnityEngine;

public interface IInventoryUI
{
    void HandleSlotClicked(InventorySlot slot, Inventory owner);
    void Refresh();
    void ToggleInventoryUI();
    
}
