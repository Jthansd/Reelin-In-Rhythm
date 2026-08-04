using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private Inventory playerInventory;

    // convenience accessors, if other scripts want "the rod" by name
    public EquipmentItem Rod => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Rod);
    public EquipmentItem Reel => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Reel);
    public EquipmentItem Line => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Line);
    public EquipmentItem Hook => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Hook);
    public EquipmentItem Bait => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Bait);

    
    private void OnEnable()
    {
        //equipmentManager.OnEquipmentChanged.AddListener(HandleEquipmentChanged);
    }

    private void OnDisable()
    {
        //equipmentManager.OnEquipmentChanged.RemoveListener(HandleEquipmentChanged);
    }

    //private void HandleEquipmentChanged(EquipmentItem.EquipmentType type)
    //{
    //    EquipmentItem item = equipmentManager.GetEquipped(type);
    //    if (item != null)
    //        ApplyEquipmentBonus(item);
    //}

    public void EquipItem(EquipmentItem equipmentItem)
    {
        EquipmentItem oldItem = equipmentManager.GetEquipped(equipmentItem.getEquipmentType);

        if (oldItem != null)
            RemoveEquipmentBonus(oldItem); // undo old item's stats before swapping

        if(equipmentItem.getEquipmentType == EquipmentItem.EquipmentType.Bait)
        {
            // Bait is consumed on use, so we don't apply its stat bonus when equipping
            // Instead, we will apply the bait's bonus when it is consumed during fishing
        }
        else
        {
            ApplyEquipmentBonus(equipmentItem); // apply new item's stats
        }

        equipmentManager.SwapEquipment(equipmentItem);
    }

    private void ApplyEquipmentBonus(EquipmentItem item)
    {
        playerStats.AdjustStat(PlayerStats.StatType.CatchStrength, item.CatchStrengthBonus);
        playerStats.AdjustStat(PlayerStats.StatType.Luck, item.LuckBonus);
        playerStats.AdjustStat(PlayerStats.StatType.RarityStrength, item.RarityBonus);
    }

    private void RemoveEquipmentBonus(EquipmentItem item)
    {
        playerStats.AdjustStat(PlayerStats.StatType.CatchStrength, -item.CatchStrengthBonus);
        playerStats.AdjustStat(PlayerStats.StatType.Luck, -item.LuckBonus);
        playerStats.AdjustStat(PlayerStats.StatType.RarityStrength, -item.RarityBonus);
    }

    private void UnequipItem(EquipmentItem.EquipmentType type)
    {
        EquipmentItem item = equipmentManager.GetEquipped(type);
        if (type == EquipmentItem.EquipmentType.Bait)
        {
            //Unequip bait without removing stat buff since it is consumed on use
           
        }
        else
        {
            RemoveEquipmentBonus(item);
        }
        equipmentManager.Unequip(type);
    }

    public void ApplyBaitBonus()
    {
        EquipmentItem bait = Bait;
        if (bait != null)
        {
            playerStats.ApplyBaitBuff(bait);
        }
    }

    public void RevertBaitBuff(EquipmentItem bait)
    {
        if (bait != null)
        {
            playerStats.RevertBaitBuff(bait);
        }
    }

    public void RevertBaitBuff()
    {
        EquipmentItem bait = Bait;
        if (bait != null)
        {
            playerStats.RevertBaitBuff(bait);
        }
    }
    public bool ConsumeBait(out EquipmentItem baitUsed)
    { 
        EquipmentItem bait = Bait;
        bool consumed = false;
        baitUsed = null;
        if (playerInventory.HasItem(bait)) //if player has bait equipped
        {
            int baitQuantity = playerInventory.GetItemQuantity(bait);
            if(baitQuantity > 0) //if player has at least 1 bait
            {
                baitUsed = bait;
                consumed = true; 
                if(baitQuantity == 1) //if player has only 1 bait
                {
                    UnequipItem(bait.getEquipmentType);
                    //player had bait and it was consumed

                }          
                
                playerInventory.RemoveItem(bait); //remove 1 bait
                

            }
           
        }
        else
        {
            consumed = false; //player does not have bait equipped
        }
            return consumed; 
    }
}