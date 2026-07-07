using System;
using System.Collections.Generic;

[Serializable]
public class InventorySlotSaveData
{
    public string itemID; // empty string means empty slot
    public int itemQuantity;
}

[Serializable]
public class SaveData
{
    public List<InventorySlotSaveData> inventorySlots = new List<InventorySlotSaveData>();
    // currency, encyclopedia progress, upgrades etc. will get added here later
}