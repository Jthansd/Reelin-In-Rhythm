using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour, IInventoryUI
{
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Inventory thisInventory;

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
        // Just viewing inventory here - no trading logic needed.
        // Left empty for now; could later show item details/tooltip on click.
    }

    public void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}