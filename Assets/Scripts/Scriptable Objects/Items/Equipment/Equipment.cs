using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : Item
{
    [SerializeField] private EquipmentType equipmentType;

    [SerializeField] private int catchStrengthBonus;

    [SerializeField] private int luckBonus;

    [SerializeField] private int rarityBonus;

    [SerializeField] private int cost;

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

    public int Cost => cost;


}
