using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentManager : MonoBehaviour
{
    [Header("Starting Equipment")]
    [SerializeField] private EquipmentItem startingRod;
    [SerializeField] private EquipmentItem startingReel;
    [SerializeField] private EquipmentItem startingLine;
    [SerializeField] private EquipmentItem startingHook;
    [SerializeField] private EquipmentItem startingBait;

    private Dictionary<EquipmentItem.EquipmentType, EquipmentItem> equippedItems
        = new Dictionary<EquipmentItem.EquipmentType, EquipmentItem>();

    public UnityEvent<EquipmentItem.EquipmentType> OnEquipmentChanged;

    private void Start()
    {
        EquipStartingItem(startingRod);
        EquipStartingItem(startingReel);
        EquipStartingItem(startingLine);
        EquipStartingItem(startingHook);
        EquipStartingItem(startingBait);
    }

    private void EquipStartingItem(EquipmentItem item)
    {
        if (item != null)
            SwapEquipment(item);
    }

    public void SwapEquipment(EquipmentItem equipmentItem)
    {
        EquipmentItem.EquipmentType type = equipmentItem.getEquipmentType;
        equippedItems[type] = equipmentItem;
        OnEquipmentChanged?.Invoke(type);
    }

    public EquipmentItem GetEquipped(EquipmentItem.EquipmentType type)
    {
        equippedItems.TryGetValue(type, out EquipmentItem item);
        return item;
    }

    public void Unequip(EquipmentItem.EquipmentType type)
    {
        equippedItems.Remove(type);
        OnEquipmentChanged?.Invoke(type);
    }
}