using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<Item> allItems;

    private Dictionary<string, Item> lookup;

    public Item GetItemByID(string id)
    {
        if (lookup == null)
            BuildLookup("ID");

        lookup.TryGetValue(id, out Item item);
        return item;
    }

    public Item GetItemByName(string name)
    {
        if (lookup == null)
            BuildLookup("Name");
        lookup.TryGetValue(name, out Item item);
        return item;
    }

    private void BuildLookup(string type)
    {
        lookup = new Dictionary<string, Item>();
        foreach (var item in allItems)
        {
            if (type == "ID")
            {
                lookup[item.ItemID] = item;
            }
            else if (type == "Name")
            {
                lookup[item.ItemName] = item;
            }
            else
            {
                //return debug fish
                Debug.LogError("Invalid lookup type specified.");
            }
        }
    }

    public List<Item> GetAllItems()
    {
        return allItems;
    }
}