using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory fromInventory;
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Inventory toInventory;

    void OnEnable()
    {
        fromInventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        fromInventory.OnInventoryChanged -= Refresh;
    }

    void Refresh()
    {
        foreach (Transform child in slotGrid)
            Destroy(child.gameObject);

        Debug.Log($"About to loop over {fromInventory.inventorySlots.Count} slots");

        foreach (var slot in fromInventory.inventorySlots)
        {
            Debug.Log($"Instantiating slot, item = {slot.item}");

            GameObject slotGO = Instantiate(slotPrefab, slotGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

            slotUI.Bind(slot, fromInventory);
            slotUI.OnSlotClicked += HandleSlotClicked;
        }
    }

    private void HandleSlotClicked(InventorySlot slot, Inventory owner)
    {
        Debug.Log($"Clicked slot. Item: {(slot.item != null ? slot.item.ItemName : "empty")}, Owner: {owner.name}");

        if (slot.item == null) return;

        // only allow moving from player inventory to shop inventory for now
        if (owner == fromInventory && fromInventory != toInventory)
        {
            inventoryManager.MoveItem(fromInventory, toInventory, slot.item, 1);
        }
    }

    public void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

    }
}