using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private Inventory playerInventory;

    public EquipmentItem Rod => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Rod);
    public EquipmentItem Reel => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Reel);
    public EquipmentItem Line => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Line);
    public EquipmentItem Hook => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Hook);
    public EquipmentItem Bait => equipmentManager.GetEquipped(EquipmentItem.EquipmentType.Bait);

    private Dictionary<string, IEquipmentEffect> activeEffects = new();

    public void EquipItem(EquipmentItem equipmentItem)
    {
        EquipmentItem oldItem = equipmentManager.GetEquipped(equipmentItem.getEquipmentType);
        if (oldItem != null)
        {
            RemoveEffect(oldItem);
        }

        string slotKey = equipmentItem.getEquipmentType.ToString();
        IEquipmentEffect effect = equipmentItem.CreateEffectInstance();
        if (effect != null)
        {
            activeEffects[slotKey] = effect;
        }

        equipmentManager.SwapEquipment(equipmentItem);
    }

    private void RemoveEffect(EquipmentItem item)
    {
        activeEffects.Remove(item.getEquipmentType.ToString());
    }

    public IEnumerable<IEquipmentEffect> GetActiveEffects() => activeEffects.Values;

    private void UnequipItem(EquipmentItem.EquipmentType type)
    {
        EquipmentItem item = equipmentManager.GetEquipped(type);
        if (item != null)
        {
            RemoveEffect(item);
        }
        equipmentManager.Unequip(type);
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
