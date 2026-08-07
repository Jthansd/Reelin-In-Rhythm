using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopDefaultUI;
    [SerializeField] private Inventory buyInventory;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Inventory cartInventory;

    //[SerializeField] private GameObject cartInventoryUI;
    [SerializeField] private GameObject leftInventoryUI;
    [SerializeField] private GameObject rightInventoryUI;
    [SerializeField] private GameObject utilUI;
    [SerializeField] private TextMeshProUGUI transactionButtonText;

    [SerializeField] private PlayerEquipment playerEquipment;

    private string currentTransactionType = "buy"; // default to buy

    private bool buyOrSell = true; // true for buy, false for sell

    public bool BuyOrSell => buyOrSell;

    public void SellItems()
    {

        int totalValue = 0;
        foreach (var slot in cartInventory.inventorySlots)//for each item in the cart
        {
            if (slot.item is FishItem fish)//if its a fish
            {
                totalValue += fish.SellValue * slot.itemQuantity; //add it to the sell value
            }
        }
        cartInventory.ClearInventory();//clear the cart
        CurrencyManager.Instance.AwardCurrency(totalValue);//award money
    }

    public void DoTransaction()
    {
        if (!buyOrSell)
        {
            SellItems();
            return;
        }
        BuyItems();
    }


    public void BuyItems()
    {
        int totalCost = 0;
        foreach (var slot in cartInventory.inventorySlots)//for each item in the cart
        {
            if (slot.item != null)//if the item is not null
            {
                totalCost += slot.item.Value * slot.itemQuantity;//add the value to the total cost
            }
        }

        if (CurrencyManager.Instance.SpendCurrency(totalCost))//if the player has enough money to spend, spend it
        {
            
            TradingManager.Instance.SetToAndFrom(playerInventory, cartInventory);//set the players inventory to receive from the cart
            foreach (var slot in cartInventory.inventorySlots)//for each item in the cart
            {
                if (slot.item != null)//if the item is not null
                {
                    if(slot.item is EquipmentItem equipmentItem)//if it is equipment
                    {
                        playerEquipment.EquipItem(equipmentItem);//equip it
                        if(equipmentItem.getEquipmentType != EquipmentItem.EquipmentType.Bait)//if its not bait
                        {
                            //do nothing, we remove from cart later
                        }
                        else //it is bait
                        {

                            TradingManager.Instance.TradeItemStack(equipmentItem, slot.itemQuantity);
                            
                        }
                    }
                    else//its not equipment
                    {
                        TradingManager.Instance.TradeItemStack(slot.item, slot.itemQuantity); //add it to the player's inventory
                    }


                }
            }
            cartInventory.ClearInventory(); //clear the cart
            Debug.Log("Items bought successfully.");
        }
        else
        {
            Debug.Log("Not enough currency to buy items.");
        }
    }


    private void Start()
    {

    }

    public void OpenShop()
    {
        SetSoloActive("default");
        MenuStateEvents.SetMenuOpen(true);

        Debug.Log("Shop opened.");
    }




    public void CloseShop()
    {
        PreventStealing();
        SetAllInactive();
        MenuStateEvents.SetMenuOpen(false);

        Debug.Log("Shop closed.");
    }

    public void OpenSellMenu()
    {
        PreventStealing();
        SetInventories(playerInventory, cartInventory);
        SetSoloActive("sell");
        buyOrSell = false;
        transactionButtonText.text = "Sell";
        Debug.Log("Sell menu opened.");
    }

    public void OpenBuyMenu()
    {
        PreventStealing();
        SetInventories(cartInventory, buyInventory);
        SetSoloActive("buy");
        buyOrSell = true;
        transactionButtonText.text = "Buy";
        Debug.Log("Buy menu opened.");
    }



    private void SetSoloActive(string activeUI)
    {
        SetAllInactive();
        if (activeUI == "buy")
       {
            leftInventoryUI.SetActive(true);
            rightInventoryUI.SetActive(true);
            utilUI.SetActive(true);
        }
       else if(activeUI == "sell")
       {
            leftInventoryUI.SetActive(true);
            rightInventoryUI.SetActive(true);
            utilUI.SetActive(true);
       }
        else if(activeUI == "default")
        {
            shopDefaultUI.SetActive(true);
        }

    }
    private void SetAllInactive()
    {
        rightInventoryUI.SetActive(false);
        shopDefaultUI.SetActive(false);
        //cartInventoryUI.SetActive(false);
        leftInventoryUI.SetActive(false);
        utilUI.SetActive(false);
    }




    private void PreventStealing()
    {
        // Return items from cart to their original inventories
        foreach (var slot in cartInventory.inventorySlots)
        {
            for (int i = 0; i <= slot.itemQuantity; i++)
            {

                if (slot.item != null)
                {
                    if (buyOrSell)
                    {
                        TradingManager.Instance.TradeItem(buyInventory, cartInventory, slot.item);
                    }
                    else
                    {
                        TradingManager.Instance.TradeItem(playerInventory, cartInventory, slot.item);
                    }
                }
            }
        }
    }

    private void SetInventories(Inventory left, Inventory right)
    {
        //get the ShopInventoryUI references from the left and right UI game objects
        ShopInventoryUI leftUI = leftInventoryUI.GetComponent<ShopInventoryUI>();
        ShopInventoryUI rightUI = rightInventoryUI.GetComponent<ShopInventoryUI>();

        //set the left and right UI objects to reference inventories that should be displayed
        //LEFT: when buying, it should be the Cart inventory. when selling, it should be the player inventory
        //RIGHT: When buying it should be the Shop inventory. When selling, it should be the player inventory

        leftUI.SetThisInventory(left);
        rightUI.SetThisInventory(right);
    }
}
