using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemDatabase allItemsDatabase;
    private Item[] allItems; // used to look up Item by ID when loading
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        Instance = this;
        allItems = allItemsDatabase.GetAllItems().ToArray();
    }

    public void Save()
    {
        SaveData data = new SaveData();

        foreach (var slot in playerInventory.inventorySlots)
        {
            InventorySlotSaveData slotData = new InventorySlotSaveData();
            slotData.itemID = slot.item != null ? slot.item.ItemID : "";
            slotData.itemQuantity = slot.itemQuantity;
            data.inventorySlots.Add(slotData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game saved to {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        playerInventory.inventorySlots.Clear();

        foreach (var slotData in data.inventorySlots)
        {
            InventorySlot slot = new InventorySlot();
            slot.item = string.IsNullOrEmpty(slotData.itemID) ? null : FindItemByID(slotData.itemID);
            slot.itemQuantity = slotData.itemQuantity;
            playerInventory.inventorySlots.Add(slot);
        }

        playerInventory.NotifyInventoryChanged(); // we'll add this method next
    }

    private Item FindItemByID(string id)
    {
        return allItemsDatabase.GetItemByID(id); 

        //Debug.LogWarning($"Item with ID {id} not found!");
        //return null;
    }
}