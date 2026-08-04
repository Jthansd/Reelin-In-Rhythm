using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum StatType { Luck, CatchStrength, RarityStrength }
    [SerializeField] private int luck; //bonus to how often the player will hook a fish
    [SerializeField] private int catchStrength; //reel speed. How fast the player can reel in fish
    [SerializeField] private int rarityStrength; //higher the rarity strength, the rarer the fish the player can hook

    public int Luck => luck;
    public int CatchStrength => catchStrength;
    public int RarityStrength => rarityStrength;

    //public static PlayerStats Instance { get; private set; }
    
    //private void Awake()
    //{
    //    Instance = this;
    //}

    public void AdjustStat(StatType bonusType, int bonusStrength)
    {
        
        ref int stat = ref GetStatReference(bonusType);
        Debug.Log(bonusType.ToString() + " stat before adjustment: " + stat);
        stat += bonusStrength;
        Debug.Log("After adjustment " + stat);
    }

    private ref int GetStatReference(StatType type)
    {
        switch (type)
        {
            case StatType.Luck:
                return ref luck;
            case StatType.CatchStrength:
                return ref catchStrength;
            case StatType.RarityStrength:
                return ref rarityStrength;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }


    public void ApplyBaitBuff(EquipmentItem bait)
    {
        AdjustStat(StatType.Luck, bait.LuckBonus);
        AdjustStat(StatType.CatchStrength, bait.CatchStrengthBonus);
        AdjustStat(StatType.RarityStrength, bait.RarityBonus);
    }

    public void RevertBaitBuff(EquipmentItem bait)
    {
        AdjustStat(StatType.Luck, -bait.LuckBonus);
        AdjustStat(StatType.CatchStrength, -bait.CatchStrengthBonus);
        AdjustStat(StatType.RarityStrength, -bait.RarityBonus);
    }



}
