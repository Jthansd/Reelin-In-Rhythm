using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private Inventory shopInventory;
    [SerializeField] private InventoryUI inventoryUI;

    

    private void Start()
    {

    }

    public void OpenShop()
    {
        PopulateInventories();
        //Debug.Log("Opening shop UI.");
        //inventoryUI.ToggleInventoryUI();
        shopUI.SetActive(true);
        Debug.Log("Shop opened.");
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        Debug.Log("Shop closed.");
    }

    private void PopulateInventories()
    {
        Debug.Log("Populating shop inventories.");

    }
}
