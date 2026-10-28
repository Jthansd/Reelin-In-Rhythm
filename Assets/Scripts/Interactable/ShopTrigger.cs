using System.Runtime.CompilerServices;
using UnityEngine;

public class ShopTrigger : MonoBehaviour, IInteractable
{

    [SerializeField] private MonoBehaviour shopScript;
    private IShop shop;

    private void Awake()
    {
        shop = shopScript as IShop;
        if (shop == null)
        {
            Debug.LogError("The shopScript does not implement IShop interface.");
        }
    }

    public void OnPlayerEnter()
    {
        Debug.Log("Player entered shop trigger");
        if (shop != null)
        {
            shop.OpenShop();
        }
    }
    public void OnPlayerExit()
    {
        if (shop != null)
        {
            shop.CloseShop();
        }
    }

}
