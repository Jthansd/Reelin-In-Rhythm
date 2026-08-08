using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private Inventory playerInventory;

    public EquipmentItem Rod => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Rod);
    public EquipmentItem Reel => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Reel);
    public EquipmentItem Line => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Line);
    public EquipmentItem Hook => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Hook);
    public EquipmentItem Bait => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Bait);

    public void EquipItem(EquipmentItem equipmentItem)
    {
        EquipmentItem oldItem = equipmentManager.GetEquipped(equipmentItem.getEquipmentType);

        if (oldItem != null)
            RemoveEquipmentBonus(oldItem);

        if (equipmentItem.getEquipmentType == EquipmentItem.EquipmentType.Bait)
        {
            // Bait is consumed on use - its bonus is applied at cast time, not equip time.
        }
        else
        {
            ApplyEquipmentBonus(equipmentItem);
        }

        equipmentManager.SwapEquipment(equipmentItem);
    }

    private void ApplyEquipmentBonus(EquipmentItem item)
    {
        string slotKey = item.getEquipmentType.ToString();
        playerStats.SetCatchStrengthMultiplier(slotKey, item.CatchStrengthMultiplier);
        playerStats.SetRarityMultiplier(slotKey, item.RarityMultiplier);
        playerStats.AdjustLuck(item.LuckBonus);
    }

    private void RemoveEquipmentBonus(EquipmentItem item)
    {
        string slotKey = item.getEquipmentType.ToString();
        playerStats.ClearCatchStrengthMultiplier(slotKey);
        playerStats.ClearRarityMultiplier(slotKey);
        playerStats.AdjustLuck(-item.LuckBonus);
    }

    private void UnequipItem(EquipmentItem.EquipmentType type)
    {
        EquipmentItem item = equipmentManager.GetEquipped(type);
        if (type != EquipmentItem.EquipmentType.Bait)
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
            playerStats.SetCatchStrengthMultiplier("Bait", bait.CatchStrengthMultiplier);
            playerStats.SetRarityMultiplier("Bait", bait.RarityMultiplier);
            playerStats.AdjustLuck(bait.LuckBonus);
        }
    }

    public void RevertBaitBuff(EquipmentItem bait)
    {
        if (bait != null)
        {
            playerStats.ClearCatchStrengthMultiplier("Bait");
            playerStats.ClearRarityMultiplier("Bait");
            playerStats.AdjustLuck(-bait.LuckBonus);
        }
    }

    public void RevertBaitBuff()
    {
        RevertBaitBuff(Bait);
    }

    public bool ConsumeBait(out EquipmentItem baitUsed)
    {
        EquipmentItem bait = Bait;
        bool consumed = false;
        baitUsed = null;
        if (playerInventory.HasItem(bait))
        {
            int baitQuantity = playerInventory.GetItemQuantity(bait);
            if (baitQuantity > 0)
            {
                baitUsed = bait;
                consumed = true;
                if (baitQuantity == 1)
                {
                    UnequipItem(bait.getEquipmentType);
                }

                playerInventory.RemoveItem(bait);
            }
        }
        else
        {
            consumed = false;
        }
        return consumed;
    }
}