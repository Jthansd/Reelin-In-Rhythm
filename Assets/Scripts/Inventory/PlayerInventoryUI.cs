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
        if (TutorialManager.Instance.HasSeen("First_Catch") && !inventoryPanel.activeSelf)
        {
            TutorialManager.Instance.ShowIfUnseen("Go_To_Shop", "The fish you caught can be sold at the shop nearby, make a visit there to see what you can buy with your newly acquired funds");
        }
    }
}