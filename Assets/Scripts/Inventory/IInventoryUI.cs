using UnityEngine;
using UnityEngine.EventSystems;

public interface IInventoryUI
{
    void HandleSlotClicked(InventorySlot slot, Inventory owner);
    void HandleSlotHoverEnter(InventorySlot slot, Inventory owner, Vector2 mousePosition);
    void HandleSlotHoverExit(InventorySlot slot, Inventory owner);
    void Refresh();
    void ToggleInventoryUI();
}