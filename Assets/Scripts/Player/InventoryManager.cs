using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private InventoryUI inventoryUI;

    public static InventoryManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;

    }

    public void ToggleInventory()
    {
        inventoryUI.ToggleInventoryUI();
    }

    public void MoveItem(Inventory from, Inventory to, Item item, int amount)
    {
        if (item == null) return;

        from.RemoveItem(item/*, amount*/);
        to.AddItem(item/*, amount*/);
    }
}
