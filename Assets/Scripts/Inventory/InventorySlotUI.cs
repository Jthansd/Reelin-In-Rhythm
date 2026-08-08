using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI qtyText;

    private InventorySlot boundSlot;
    private Inventory ownerInventory;

    public event Action<InventorySlot, Inventory> OnSlotClicked;
    public event Action<InventorySlot, Inventory, Vector2> OnSlotHoverEnter;
    public event Action<InventorySlot, Inventory> OnSlotHoverExit;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;
        OnSlotHoverEnter?.Invoke(boundSlot, ownerInventory, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnSlotHoverExit?.Invoke(boundSlot, ownerInventory);
    }
}