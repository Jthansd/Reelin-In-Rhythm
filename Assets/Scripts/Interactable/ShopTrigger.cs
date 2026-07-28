using System.Runtime.CompilerServices;
using UnityEngine;

public class ShopTrigger : MonoBehaviour, IInteractable
{

    [SerializeField] private Shop shop;

    public void OnPlayerEnter()
    {
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
