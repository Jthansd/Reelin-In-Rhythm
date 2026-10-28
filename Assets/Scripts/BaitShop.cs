using UnityEngine;

public class BaitShop : MonoBehaviour, IShop
{
    [SerializeField] private GameObject shopUI;
    private bool isOpen = false;
    void IShop.CloseShop()
    {
        if (isOpen)
        {
            shopUI.SetActive(false);
            isOpen = false;
            MenuStateEvents.SetMenuOpen(false);
            Debug.Log("Bait Shop Closed");
        }
    }

    void IShop.OpenShop()
    {
        if(!isOpen)
        {
            shopUI.SetActive(true);
            isOpen = true;
            MenuStateEvents.SetMenuOpen(true);
            Debug.Log("Bait Shop Opened");
        }
    }

    public void CloseShop()
    {
        if (isOpen)
        {
            shopUI.SetActive(false);
            isOpen = false;
            MenuStateEvents.SetMenuOpen(false);
            Debug.Log("Bait Shop Closed");
        }
    }

}
