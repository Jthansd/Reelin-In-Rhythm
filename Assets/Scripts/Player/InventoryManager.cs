using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;

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


}
