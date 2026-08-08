using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class EquipmentItem : Item
{
    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] private float catchStrengthMultiplier = 1.0f; // e.g. 1.15 = +15%
    [SerializeField] private int luckBonus = 0; // stays additive
    [SerializeField] private float rarityMultiplier = 1.0f;

    public enum EquipmentType
    {
        Rod,
        Reel,
        Bait,
        Line,
        Hook
    }

    public EquipmentType getEquipmentType => equipmentType;
    public float CatchStrengthMultiplier => catchStrengthMultiplier;
    public int LuckBonus => luckBonus;
    public float RarityMultiplier => rarityMultiplier;
}