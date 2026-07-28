using UnityEngine;

//public enum InventoryUIMode { Buy, Sell, Cart, Return }
public class ShopInventoryUI : MonoBehaviour, IInventoryUI
{
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Inventory thisInventory;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Inventory cartInventory;
    [SerializeField] private Inventory buyInventory;
    [SerializeField] private Inventory sellInventory;
    //[SerializeField] private InventoryUIMode mode = InventoryUIMode.Buy;
    [SerializeField] private Shop shop;

    

    void OnEnable()
    {
        thisInventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        thisInventory.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        foreach (Transform child in slotGrid)
            Destroy(child.gameObject);

        foreach (var slot in thisInventory.inventorySlots)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Bind(slot, thisInventory);
            slotUI.OnSlotClicked += HandleSlotClicked;
        }
    }

    public void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {

        Debug.Log("You clicked a slot from " + owner.ToString());
        if (slot.item == null) return;
        if (owner != thisInventory) return;

        if(owner == thisInventory && slot.item != null)
        {
            Debug.Log("You clicked a slot from " + owner.ToString() + " with item: " + slot.item.name);
        }

        if (owner == playerInventory)
        {
            if (shop.BuyOrSell)
            {
                Debug.Log("Player should not have clicked player inventory while buying items");
            }
            else if (!shop.BuyOrSell)
            {
                Debug.Log("Player clicked player inventory while selling items");
                //Move items from player to cart
                TradingManager.Instance.TradeItem(cartInventory, playerInventory, slot.item);
            }
        }
        else if (owner == buyInventory)
        {
            Debug.Log("buyOrSell is " + shop.BuyOrSell.ToString());
            if (shop.BuyOrSell)
            {
                Debug.Log("Player clicked buy inventory while buying items");
                //Move items from buy inventory to cart
                TradingManager.Instance.TradeItem(cartInventory, buyInventory, slot.item);
            }
            else if (!shop.BuyOrSell)
            {
                Debug.Log("Player should not have clicked buy inventory while selling items");
            }
        }
        else if (owner == cartInventory)
        {
            if (shop.BuyOrSell)
            {
                Debug.Log("Player clicked cart inventory while buying items");
                //Return items from cart to buy inventory
                TradingManager.Instance.TradeItem(buyInventory, cartInventory, slot.item);
            }
            else if (!shop.BuyOrSell)
            {
                Debug.Log("Player clicked cart inventory while selling items");
                //Return items from cart to player inventory
                
                TradingManager.Instance.TradeItem(playerInventory, cartInventory, slot.item);
            }

        }
    }

    public void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void SetThisInventory(Inventory inventory)
    {
        thisInventory = inventory;
        thisInventory.NotifyInventoryChanged();
    }
  
}