using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private static int maxInventorySize = 20;

    public event Action OnInventoryChanged;

    private void Start()
    {
        populateInventory();
    }


    private void populateInventory()
    {
        for (int i = 0; i < maxInventorySize; i++)
        {
            InventorySlot slot = new InventorySlot();
            inventorySlots.Add(slot);
        }
        OnInventoryChanged?.Invoke(); // fire once after populating, not per-slot
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item == null)
            {
                inventorySlots[i].item = item;
                inventorySlots[i].itemQuantity = 1;
                OnInventoryChanged?.Invoke();
                return;
            }
            else if (inventorySlots[i].item == item)
            {
                inventorySlots[i].itemQuantity++;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        Debug.LogWarning("Inventory is full!");
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item == item)
            {
                inventorySlots[i].itemQuantity--;
                if (inventorySlots[i].itemQuantity <= 0)
                {
                    inventorySlots[i].item = null;
                }
                OnInventoryChanged?.Invoke();
                return;
            }
        }
    }

    public bool HasItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item == item)
            {
                return true;
            }
        }
        return false;
    }

    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}
