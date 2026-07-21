using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI qtyText;

    private InventorySlot boundSlot;
    private Inventory ownerInventory;

    public event Action<InventorySlot, Inventory> OnSlotClicked;

    public void Bind(InventorySlot slot, Inventory owner)
    {
        boundSlot = slot;
        ownerInventory = owner;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(boundSlot, ownerInventory);
    }
}