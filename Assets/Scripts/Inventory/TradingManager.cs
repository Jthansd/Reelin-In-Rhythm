using UnityEngine;

public class TradingManager : MonoBehaviour
{
    private Inventory fromInventory;
    private Inventory toInventory;
    public static TradingManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
    }


    public void TradeItem(Item item)
    {
        if (item == null) return;
        if (fromInventory != null && toInventory != null)
        {
            fromInventory.RemoveItem(item);
            toInventory.AddItem(item);
        }
    }
  
    public void TradeItem(Inventory to, Inventory from, Item item)
    {
        if (item == null) return;
        if (from != null && to != null)
        {
            from.RemoveItem(item);
            to.AddItem(item);
        }
    }

    public void SetToAndFrom(Inventory to, Inventory from)
    {
        fromInventory = from;
        toInventory = to;
    }
}