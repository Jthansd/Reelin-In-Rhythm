using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopDefaultUI;
    [SerializeField] private GameObject shopBrowseUI; // the single scroll view panel, shown for all 3 tabs
    [SerializeField] private EquipmentShopUI equipmentShopUI;
    [SerializeField] private StatBoostShopUI statBoostShopUI;
    // [SerializeField] private CosmeticShopUI cosmeticShopUI; // once it exists

    private bool isOpen = false;

    public void OpenShop()
    {
        if (!isOpen)
        {
            isOpen = true;
            MenuStateEvents.SetMenuOpen(true);
        }
        SetSoloActive("default");
        SwitchMenu("equipment");
        Debug.Log("Shop opened.");
    }

    public void CloseShop()
    {
        if (!isOpen) return;
        isOpen = false;
        SetAllInactive();
        MenuStateEvents.SetMenuOpen(false);
        Debug.Log("Shop closed.");
    }

    private void SetSoloActive(string activeUI)
    {
        SetAllInactive();
        if (activeUI == "default")
        {
            shopDefaultUI.SetActive(true);
        }
    }

    private void SetAllInactive()
    {
        shopDefaultUI.SetActive(false);
    }

    public void SwitchMenu(string menuName)
    {
        

        switch (menuName.ToLower())
        {
            case "equipment":
                equipmentShopUI.Refresh();
                return;
            case "upgrades":
                statBoostShopUI.Refresh();
                return;
            case "cosmetics":
                // cosmeticShopUI.Refresh();
                return;
        }
    }
}