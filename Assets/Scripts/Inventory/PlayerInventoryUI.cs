using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInventoryUI : MonoBehaviour, IInventoryUI
{
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Inventory thisInventory;
    [SerializeField] private GameObject slotPopupPrefab;
    [SerializeField] private RectTransform canvasTransform;

    private GameObject itemDetails;

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


    public void HandleSlotHoverEnter(InventorySlot slot, Inventory owner, Vector2 mousePosition)
    {
        if (slot.item == null) return;

        Debug.Log("Tool Tip Activated");
        itemDetails = Instantiate(slotPopupPrefab, canvasTransform);

        RectTransform itemDetailsRect = itemDetails.GetComponent<RectTransform>();
        RectTransform referenceRect = itemDetails.transform.Find("TransformReference").GetComponent<RectTransform>();

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

        FillItemPopup(itemDetails, slot);
        itemDetails.SetActive(true);
    }

    public void FillItemPopup(GameObject itemDetails, InventorySlot slot)
    {
        TextMeshProUGUI nameText = itemDetails.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText = itemDetails.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceText = itemDetails.transform.Find("SellPriceText").GetComponent<TextMeshProUGUI>();

        nameText.text = slot.item.ItemName;
        descText.text = slot.item.ItemDescription;
        if(slot.item is FishItem fish)
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
        if (itemDetails == null) return;

        Destroy(itemDetails);
        itemDetails = null;
        Debug.Log("Tool Tip Deactivated");
    }

    public void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {
        // Just viewing inventory here - no trading logic needed.
        // Left empty for now; could later show item details/tooltip on click.
        Debug.Log("Player Inventory Clicked");
    }

    public void ToggleInventoryUI()
    {
        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);
        MenuStateEvents.SetMenuOpen(newState);
    }
}