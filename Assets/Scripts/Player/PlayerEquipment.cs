using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private EquipmentManager equipmentManager;

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

        equipmentManager.SwapEquipment(equipmentItem);
        ApplyEquipmentBonus(equipmentItem); // apply new item's stats
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
}