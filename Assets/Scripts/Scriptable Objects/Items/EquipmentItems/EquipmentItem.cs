using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class EquipmentItem : Item
{
    [SerializeField] private EquipmentType equipmentType;

    [Header("Special Effect")]
    [SerializeField] private EquipmentEffectType effectType = EquipmentEffectType.None;
    [SerializeField] private int effectIntParam;
    [SerializeField] private float effectFloatParam;
    [SerializeField] private FishItem.Rarity effectRarityParam;
    [SerializeField] private FishItem.FishSize effectFishSizeParam;

    public enum EquipmentType
    {
        Rod, Reel, Bait, Line, Hook
    }

    public EquipmentType getEquipmentType => equipmentType;

    public static List<EquipmentType> GetAllEquipmentTypes()
    {
        return Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>().ToList();
    }

    public IEquipmentEffect CreateEffectInstance()
    {
        switch (effectType)
        {
            case EquipmentEffectType.MissShield:
                return new MissShieldEffect(effectIntParam);
            case EquipmentEffectType.HookRarity:
                return new HookRarityEffect(effectRarityParam);
            case EquipmentEffectType.HookSize:
                return new HookSizeEffect(effectFishSizeParam);
            //case EquipmentEffectType.BoostSellPrice:
            //    return new BoostSellPriceEffect(effectFloatParam);
            default:
                return null;
        }
    }
}

public enum EquipmentEffectType
{
    None,
    MissShield,
    HookRarity,
    HookSize,
    BoostSellPrice,


}