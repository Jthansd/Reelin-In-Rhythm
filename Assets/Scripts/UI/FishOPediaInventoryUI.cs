using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishOPediaInventoryUI : MonoBehaviour, IInventoryUI
{
    [SerializeField] GameObject fishOPedia;
    [SerializeField] Inventory LeftPageInventory;
    [SerializeField] Inventory RightPageInventory;
    [SerializeField] Inventory thisInventory;
    [SerializeField] Inventory FishOPediaInventory;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject toolTipPrefab;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private GameObject inventoryPanel;

    private GameObject toolTip;

    private FishItem.Rarity currentRarity;

    public void SetRarityAndRefresh(FishItem.Rarity rarity)
    {
        currentRarity = rarity;
        Refresh();
    }

    public void UpdateInventory()
    {
        thisInventory.ClearInventory();
        List<Item> pageItems = FishOPedia.Instance.GetAllObtainableFishOfRarity(currentRarity);

        foreach (var item in pageItems)
        {
            thisInventory.AddItem(item);
        }
    }

    public void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {
        //do nothing for now
    }

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
        thisInventory.OnInventoryChanged -= Refresh; // prevent re-entry during rebuild
        UpdateInventory();
        thisInventory.OnInventoryChanged += Refresh;

        foreach (Transform child in slotGrid)
            Destroy(child.gameObject);

        foreach (var slot in thisInventory.inventorySlots)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Bind(slot, thisInventory, true);
            slotUI.OnSlotClicked += HandleSlotClicked;
            slotUI.OnSlotHoverEnter += HandleSlotHoverEnter;
            slotUI.OnSlotHoverExit += HandleSlotHoverExit;
        }
    }

    public void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}

   
