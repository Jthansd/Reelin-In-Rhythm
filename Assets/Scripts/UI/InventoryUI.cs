using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventoryPanel;

    void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
    }

    void Refresh()
    {
        foreach (Transform child in slotGrid)
            Destroy(child.gameObject);

        Debug.Log($"About to loop over {inventory.inventorySlots.Count} slots");

        foreach (var slot in inventory.inventorySlots)
        {
            Debug.Log($"Instantiating slot, item = {slot.item}");
            GameObject slotGO = Instantiate(slotPrefab, slotGrid);

            Image icon = slotGO.transform.Find("ItemIcon").GetComponent<Image>();
            TextMeshProUGUI qtyText = slotGO.transform.Find("ItemCountText").GetComponent<TextMeshProUGUI>();

            if (slot.item != null)
            {
                icon.sprite = slot.item.Icon;
                icon.enabled = true;
                qtyText.text = slot.itemQuantity > 1 ? slot.itemQuantity.ToString() : "";
            }
            else
            {
                icon.enabled = false;
                qtyText.text = "";
            }
        }
    }

    public void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

    }
}