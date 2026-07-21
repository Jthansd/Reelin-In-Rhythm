using UnityEngine;

public class Shop : MonoBehaviour
{
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
        shopUI.SetActive(true);
        //shopInventory.AddItem()
        
        Debug.Log("Shop opened.");
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        Debug.Log("Shop closed.");
    }

    public void SellItems()
    {
        int totalValue = 0;
        foreach(var slot in shopInventory.inventorySlots)
        {
            if(slot.item != null)
            {
                totalValue += slot.item.ItemValue * slot.itemQuantity;

            }
        }
        shopInventory.ClearInventory();
        CurrencyManager.Instance.AwardCurrency(totalValue);
    }
}
