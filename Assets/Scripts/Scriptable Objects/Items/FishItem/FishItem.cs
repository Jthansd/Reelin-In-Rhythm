using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FishItem", menuName = "Scriptable Objects/FishItem")]
public class FishItem : Item
{
    public enum Rarity {
        Common, //0
        Uncommon, // 10
        Rare, //25
        SuperRare, //50
        Legendary //75
    }

    public enum FishSize
    {
        Tiny,
        Small,
        Medium,
        Large,
        Giant
    }



    [SerializeField] private int baseValue;
    [SerializeField] private float customDifficultyMultiplier = 1.0f;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private List<Rarity> possibleRarities;

    private float baseDifficulty = 10;
    public int BaseValue => baseValue;
    public float BaseDifficulty => baseDifficulty;
    public float CustomDifficultyMultiplier => customDifficultyMultiplier;
    public List<Rarity> PossibleRarities => possibleRarities;
    public float MaxSize => maxSize;
    public float MinSize => minSize;
    public static Color RarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return Color.blue;
            case Rarity.Uncommon:
                return Color.green;
            case Rarity.Rare:
                return Color.orange;
            case Rarity.SuperRare:
                return new Color(0.7f, 0.3f, 1f);
            case Rarity.Legendary:
                return new Color(1f, 0.8f, 0.1f);
            default:
                return Color.grey;
        }
    }

    public static List<Rarity> GetPlayerFacingRarities()
    {
        return Enum.GetValues(typeof(Rarity)).Cast<Rarity>().ToList();
    }

    public int GetRarityMultiplier(FishItem.Rarity rarityType)
    {
        switch (rarityType)
        {
            case Rarity.Common:
                return 0;
            case Rarity.Uncommon:
                return 1;
            case Rarity.Rare:
                return 2;
            case Rarity.SuperRare:
                return 3;
            case Rarity.Legendary:
                return 4;
            default:
                return 0;
        }
    }

    public FishItem.FishSize GetFishSizeFromSizeValue(float fishSizeValue)
    {
        //determine fish size based on value

        //return medium for now
        return FishItem.FishSize.Medium;
    }

    public float GetSizeLowerBound(FishSize size)
    {
        int numSizes = Enum.GetValues(typeof(FishSize)).Length;
        int sizeIndex = (int)size;

        float bandWidth = (MaxSize - MinSize) / numSizes;
        return MinSize + (bandWidth * sizeIndex);
    }

    public float GetSizeUpperBound(FishSize size)
    {
        return GetSizeLowerBound(size) + ((MaxSize - MinSize) / System.Enum.GetValues(typeof(FishSize)).Length);
    }

}
