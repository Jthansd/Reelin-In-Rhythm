using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject sellUI;
    [SerializeField] private GameObject buyUI;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private Inventory shopInventory;
    [SerializeField] private Inventory playerInventory; 
    [SerializeField] private InventoryUI shopInventoryUI;
    [SerializeField] private InventoryUI playerInventoryUI;



    private void Start()
    {

    }

    public void OpenShop()
    {
        SetSoloActive("shop");

        Debug.Log("Shop opened.");
    }

    public void CloseShop()
    {

        SetAllInactive();
        Debug.Log("Shop closed.");
    }

    public void OpenSellMenu()
    {
        SetSoloActive("sell");
        Debug.Log("Sell menu opened.");
    }

    public void OpenBuyMenu()
    {
        SetSoloActive("buy");
        Debug.Log("Buy menu opened.");
    }



    public void SellItems()
    {
        int totalValue = 0;
        foreach(var slot in shopInventory.inventorySlots)
        {
            if(slot.item is FishItem fish)
            {
                totalValue += fish.SellValue * slot.itemQuantity;

            }
        }
        shopInventory.ClearInventory();
        CurrencyManager.Instance.AwardCurrency(totalValue);
    }

    public void BuyItems()
    {
        int totalCost = 0;
        foreach(var slot in playerInventory.inventorySlots)
        {
            if(slot.item is Equipment equipment)
            {
                totalCost += equipment.Cost * slot.itemQuantity;
            }
        }

        CurrencyManager.Instance.SpendCurrency(totalCost);
        Debug.Log("Buying items...");
    }

    private void SetSoloActive(string activeUI)
    {
        sellUI.SetActive(activeUI == "sell");
        buyUI.SetActive(activeUI == "buy");
        shopUI.SetActive(activeUI == "shop");
    }

    private void SetAllInactive()
    {
        sellUI.SetActive(false);
        buyUI.SetActive(false);
        shopUI.SetActive(false);
    }
}
