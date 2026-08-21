using System.Collections.Generic;
using UnityEngine;

public class FishOPediaInventoryUI : InventoryDisplayBase
{
    [SerializeField] FishOPediaManager FishOPediaManager;

    private void UpdateInventory()
    {
        if (FishOPedia.Instance == null)
        {
            Debug.LogWarning("FishOPediaInventoryUI: FishOPedia.Instance not ready yet, skipping update.");
            return;
        }

        thisInventory.ClearInventory();
        List<Item> pageItems = FishOPedia.Instance.GetAllObtainableFish();

        foreach (var item in pageItems)
        {
            thisInventory.AddItem(item);
        }
    }

    public override void Refresh()
    {
        thisInventory.OnInventoryChanged -= Refresh; // prevent re-entry while UpdateInventory mutates thisInventory
        UpdateInventory();
        thisInventory.OnInventoryChanged += Refresh;

        base.Refresh(); // now safe - renders slots from the settled inventory state
    }

    protected override bool UseDiscoveryTint() => true;

    public override void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {
        CloseTooltip();
        // Just viewing the encyclopedia here - no interaction needed on click.
        ViewEntry(slot);
    }

    public void ViewEntry(InventorySlot slot)
    {
        if (slot.IsCaughtFish)
        {
            Debug.LogWarning("Expected a FishItem");
            return;
        }
        if(slot.item is FishItem fish)
        {
            FishOPediaManager.SwitchToEntry(fish);
        }
    }
}