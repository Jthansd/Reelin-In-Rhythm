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

    [Header("Unlock Condition")]
    [SerializeField] private UnlockConditionType unlockType = UnlockConditionType.None;
    [SerializeField] private EquipmentItem requiredPreviousItem; // used by RequiresPreviousItemOwned
    [SerializeField] private string requiredObjectiveId; // used by RequiresQuestComplete

    public enum EquipmentType
    {
        Rod, Reel, Bait, Line, Hook, Lure
    }

    public enum UnlockConditionType
    {
        None,
        Purchase,
        Objective
    }

    public EquipmentType getEquipmentType => equipmentType;
    public UnlockConditionType getUnlockType => unlockType;

    public IUnlockCondition CreateUnlockRequirement(PlayerEquipment playerEquipment)
    {
        switch (unlockType)
        {
            case UnlockConditionType.Purchase:
                if (requiredPreviousItem != null)
                {
                    string id = requiredPreviousItem.ItemID;
                    return new PurchaseUnlockCondition(id, playerEquipment);
                }
                return null;
            case UnlockConditionType.Objective:
                if(requiredObjectiveId != null)
                {
                    return new ObjectiveUnlockCondition(requiredObjectiveId);
                }
                return null;
            default:
                return null;
        }
    }

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