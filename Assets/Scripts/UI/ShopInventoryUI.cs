using TMPro;
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
    [SerializeField] private GameObject toolTipPrefab;
    [SerializeField] private RectTransform canvasTransform;

    private GameObject toolTip;

    public void HandleSlotHoverEnter(InventorySlot slot, Inventory owner, Vector2 mousePosition)
    {
        if (slot.item == null) return;

        Debug.Log("Tool Tip Activated");
        toolTip = Instantiate(toolTipPrefab, canvasTransform);

        RectTransform itemDetailsRect = toolTip.GetComponent<RectTransform>();
        RectTransform referenceRect = toolTip.transform.Find("TransformReference").GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition,
            null,
            out Vector2 targetLocalPoint
        );

        // Offset between the root's anchored position and the reference point's local position,
        // so we can shift the root such that TransformReference lands exactly on the cursor.
        Vector2 referenceOffset = (Vector2)referenceRect.localPosition;
        itemDetailsRect.anchoredPosition = targetLocalPoint - referenceOffset;

        FillItemPopup(toolTip, slot);
        toolTip.SetActive(true);
    }

    public void FillItemPopup(GameObject itemDetails, InventorySlot slot)
    {
        TextMeshProUGUI nameText = itemDetails.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText = itemDetails.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceText = itemDetails.transform.Find("SellPriceText").GetComponent<TextMeshProUGUI>();

        nameText.text = slot.item.ItemName;
        descText.text = slot.item.ItemDescription;
        if (slot.item is FishItem fish)
        {
            priceText.text = fish.SellValue.ToString();
        }
        else
        {
            priceText.text = slot.item.Value.ToString();
        }

    }
    public void HandleSlotHoverExit(InventorySlot slot, Inventory owner)
    {
        if (toolTip == null) return;

        Destroy(toolTip);
        toolTip = null;
        Debug.Log("Tool Tip Deactivated");
    }






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
            slotUI.OnSlotHoverEnter += HandleSlotHoverEnter;
            slotUI.OnSlotHoverExit += HandleSlotHoverExit;
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