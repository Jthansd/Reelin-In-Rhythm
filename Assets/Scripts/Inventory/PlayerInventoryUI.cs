public class PlayerInventoryUI : InventoryDisplayBase
{
    public override void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {
        CloseTooltip();
        
    }

    public override void ToggleInventoryUI()
    {
        base.ToggleInventoryUI();
        MenuStateEvents.SetMenuOpen(inventoryPanel.activeSelf);
    }
}