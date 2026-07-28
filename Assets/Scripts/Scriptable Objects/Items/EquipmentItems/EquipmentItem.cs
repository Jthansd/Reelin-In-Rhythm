using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class EquipmentItem : Item
{
    [SerializeField] private EquipmentType equipmentType;

    [SerializeField] private int catchStrengthBonus;

    [SerializeField] private int luckBonus;

    [SerializeField] private int rarityBonus;


    public enum EquipmentType
    {
        Rod,
        Reel,
        Bait,
        Line,
        Hook
    }

    public EquipmentType getEquipmentType => equipmentType;

    public int CatchStrengthBonus => catchStrengthBonus;

    public int LuckBonus => luckBonus;

    public int RarityBonus => rarityBonus;



}
