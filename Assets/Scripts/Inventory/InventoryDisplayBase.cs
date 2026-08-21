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
            slotUI.Bind(slot, thisInventory, UseDiscoveryTint(), DiscoveryRarityContext());
            slotUI.OnSlotClicked += HandleSlotClicked;
            slotUI.OnSlotHoverEnter += HandleSlotHoverEnter;
            slotUI.OnSlotHoverExit += HandleSlotHoverExit;
        }
    }

    // Only FishOPediaInventoryUI needs discovery tinting - defaults to off for everyone else.
    protected virtual bool UseDiscoveryTint() => false;

    // The rarity being displayed, for species (non-CaughtFish) slots - null means "any rarity" fallback.
    protected virtual FishItem.Rarity? DiscoveryRarityContext() => null;

    public void HandleSlotHoverEnter(InventorySlot slot, Inventory owner, Vector2 mousePosition)
    {
        CloseTooltip();

        if (slot.item == null && !slot.IsCaughtFish) return;

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
        
        if (slot.IsCaughtFish)
        {
            CaughtFish fish = slot.caughtFish;

            bool isHiddenFish = UseDiscoveryTint()
                && FishOPedia.Instance != null
                && !FishOPedia.Instance.IsDiscovered(fish.species, fish.rarity);

            if (isHiddenFish)
            {
                nameText.text = "???";
                descText.text = "???";
                priceText.text = "???";
                return;
            }

            nameText.text = fish.species.ItemName;
            descText.text = $"{fish.rarity} - {fish.size:F1}cm\n{fish.species.ItemDescription}";
            priceText.text = fish.sellPrice.ToString();
            return;
        }

        FishItem.Rarity? discoveryRarityContext = DiscoveryRarityContext();
        bool isHiddenItem = UseDiscoveryTint()
            && slot.item is FishItem staticFish
            && FishOPedia.Instance != null
            && !(discoveryRarityContext.HasValue
                ? FishOPedia.Instance.IsDiscovered(staticFish, discoveryRarityContext.Value)
                : FishOPedia.Instance.IsDiscovered(staticFish));

        if (isHiddenItem)
        {
            nameText.text = "???";
            descText.text = "???";
            priceText.text = "???";
            return;
        }

        nameText.text = slot.item.ItemName;
        descText.text = slot.item.ItemDescription;
        priceText.text = slot.item is FishItem discoveredFish ? discoveredFish.BaseValue.ToString() : slot.item.Value.ToString();
    }

    public virtual void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        CloseTooltip();
    }

    // Each subclass decides what a click actually does.
    public abstract void HandleSlotClicked(InventorySlot slot, Inventory owner);
}