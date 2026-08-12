using UnityEngine;

public class ShopInventoryUI : InventoryDisplayBase
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Inventory cartInventory;
    [SerializeField] private Inventory buyInventory;
    [SerializeField] private Inventory sellInventory;
    [SerializeField] private Shop shop;

    public override void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {
        CloseTooltip();

        Debug.Log("You clicked a slot from " + owner.ToString());
        if (slot.item == null) return;
        if (owner != thisInventory) return;

        if (owner == thisInventory && slot.item != null)
        {
            Debug.Log("You clicked a slot from " + owner.ToString() + " with item: " + slot.item.name);
        }

        if (owner == playerInventory)
        {
            if (shop.BuyOrSell)
            {
                Debug.Log("Player should not have clicked player inventory while buying items");
            }
            else
            {
                Debug.Log("Player clicked player inventory while selling items");
                TradingManager.Instance.TradeItem(cartInventory, playerInventory, slot.item);
            }
        }
        else if (owner == buyInventory)
        {
            Debug.Log("buyOrSell is " + shop.BuyOrSell.ToString());
            if (shop.BuyOrSell)
            {
                Debug.Log("Player clicked buy inventory while buying items");
                TradingManager.Instance.TradeItem(cartInventory, buyInventory, slot.item);
            }
            else
            {
                Debug.Log("Player should not have clicked buy inventory while selling items");
            }
        }
        else if (owner == cartInventory)
        {
            if (shop.BuyOrSell)
            {
                Debug.Log("Player clicked cart inventory while buying items");
                TradingManager.Instance.TradeItem(buyInventory, cartInventory, slot.item);
            }
            else
            {
                Debug.Log("Player clicked cart inventory while selling items");
                TradingManager.Instance.TradeItem(playerInventory, cartInventory, slot.item);
            }
        }
    }

    public void SetThisInventory(Inventory inventory)
    {
        thisInventory = inventory;
        thisInventory.NotifyInventoryChanged();
    }
}