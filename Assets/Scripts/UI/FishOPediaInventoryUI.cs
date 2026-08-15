using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishOPediaInventoryUI : InventoryDisplayBase
{
    private FishItem.Rarity currentRarity;
    [SerializeField] TextMeshProUGUI rarityText;

    public void SetRarityAndRefresh(FishItem.Rarity rarity)
    {
        rarityText.text = rarity.ToString();
        currentRarity = rarity;
        Refresh();
    }

    private void UpdateInventory()
    {
        if (FishOPedia.Instance == null)
        {
            Debug.LogWarning("FishOPediaInventoryUI: FishOPedia.Instance not ready yet, skipping update.");
            return;
        }

        thisInventory.ClearInventory();
        List<Item> pageItems = FishOPedia.Instance.GetAllObtainableFishOfRarity(currentRarity);

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
    }
}