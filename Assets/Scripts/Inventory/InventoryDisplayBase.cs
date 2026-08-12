using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class InventoryDisplayBase : MonoBehaviour, IInventoryUI
{
    [SerializeField] protected Transform slotGrid;
    [SerializeField] protected GameObject slotPrefab;
    [SerializeField] protected GameObject inventoryPanel;
    [SerializeField] protected Inventory thisInventory;
    [SerializeField] protected GameObject toolTipPrefab;
    [SerializeField] protected RectTransform canvasTransform;

    protected GameObject toolTip;

    protected virtual void OnEnable()
    {
        thisInventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    protected virtual void OnDisable()
    {
        thisInventory.OnInventoryChanged -= Refresh;
        CloseTooltip();
    }

    public virtual void Refresh()
    {
        foreach (Transform child in slotGrid)
            Destroy(child.gameObject);

        foreach (var slot in thisInventory.inventorySlots)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Bind(slot, thisInventory, UseDiscoveryTint());
            slotUI.OnSlotClicked += HandleSlotClicked;
            slotUI.OnSlotHoverEnter += HandleSlotHoverEnter;
            slotUI.OnSlotHoverExit += HandleSlotHoverExit;
        }
    }

    // Only FishOPediaInventoryUI needs discovery tinting - defaults to off for everyone else.
    protected virtual bool UseDiscoveryTint() => false;

    public void HandleSlotHoverEnter(InventorySlot slot, Inventory owner, Vector2 mousePosition)
    {
        CloseTooltip();

        if (slot.item == null) return;

        toolTip = Instantiate(toolTipPrefab, canvasTransform);
        RectTransform toolTipRect = toolTip.GetComponent<RectTransform>();
        RectTransform referenceRect = toolTip.transform.Find("TransformReference").GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition,
            null,
            out Vector2 targetLocalPoint
        );

        Vector2 referenceOffset = (Vector2)referenceRect.localPosition;
        toolTipRect.anchoredPosition = targetLocalPoint - referenceOffset;

        FillItemPopup(toolTip, slot);
        toolTip.SetActive(true);
    }

    public void HandleSlotHoverExit(InventorySlot slot, Inventory owner)
    {
        CloseTooltip();
    }

    protected void CloseTooltip()
    {
        if (toolTip == null) return;
        Destroy(toolTip);
        toolTip = null;
    }

    protected void FillItemPopup(GameObject itemDetails, InventorySlot slot)
    {
        TextMeshProUGUI nameText = itemDetails.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText = itemDetails.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceText = itemDetails.transform.Find("SellPriceText").GetComponent<TextMeshProUGUI>();

        bool isHiddenFish = UseDiscoveryTint()
            && slot.item is FishItem fish
            && FishOPedia.Instance != null
            && !FishOPedia.Instance.IsDiscovered(fish);

        if (isHiddenFish)
        {
            nameText.text = "???";
            descText.text = "???";
            priceText.text = "???";
            return;
        }

        nameText.text = slot.item.ItemName;
        descText.text = slot.item.ItemDescription;
        priceText.text = slot.item is FishItem discoveredFish ? discoveredFish.SellValue.ToString() : slot.item.Value.ToString();
    }

    public virtual void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        CloseTooltip();
    }

    // Each subclass decides what a click actually does.
    public abstract void HandleSlotClicked(InventorySlot slot, Inventory owner);
}