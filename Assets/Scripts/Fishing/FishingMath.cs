using UnityEditor;
using UnityEngine;

public static class FishingMath
{

    private const float BaselineCatchStrength = 10f;
    private const float FishScalingFactor = 1.75f;
    private static readonly float[] RarityMultipliers = { 1.0f, 1.25f, 1.5f, 1.75f, 2.0f };
    public static float CalculateHitProgress(float playerCatchStrength, float fishDifficulty, float hitsNeededAtParity)
    {
        return (playerCatchStrength / fishDifficulty) / hitsNeededAtParity;
    }

    public static float CalculateFishDifficultyProduct(float baseDifficulty, int fishRarityFactor, float perFishMultiplier, FishItem.FishSize fishSize)
    {
        float sizeAdjustmentFactor = 0.1f;
        int numSizes = System.Enum.GetValues(typeof(FishItem.FishSize)).Length;
        int roundedSizes = Mathf.FloorToInt(numSizes / 2);
        int index = (int)fishSize;
        float sizeFactor = (-sizeAdjustmentFactor * roundedSizes) + (sizeAdjustmentFactor * index);
        return baseDifficulty * Mathf.Pow(FishScalingFactor, fishRarityFactor) * perFishMultiplier * (1f + sizeFactor);
    }

    public static float CalculateNotesNeededAtParity(float percentagePassing, int noteCount)
    {
        return noteCount * percentagePassing;
    }

    public static float CalculateMissPenalty(float fishDifficulty, float hitsNeededAtParity, float severity)
    {
        return (fishDifficulty / (BaselineCatchStrength * hitsNeededAtParity)) * severity;
    }

    public static float CalculateTutorialPause(float timing, float bpm, int beatsTillHit)
    {
        return (timing);
    }

    public static float CalculateFishSize(float minSize, float maxSize)
    {
        float randomValue = Random.Range(minSize, maxSize);
        float size = Mathf.Round(randomValue * 10f) / 10f;
        return size;
    }

    public static FishItem.FishSize GetFishSizeCategory(float fishSizeValue, float maxSize)
    {
        float percentage = fishSizeValue / maxSize;
        int numSizes = System.Enum.GetValues(typeof(FishItem.FishSize)).Length;

        int index = Mathf.FloorToInt(percentage * numSizes);
        index = Mathf.Clamp(index, 0, numSizes - 1); // safety net, explained below

        return (FishItem.FishSize)index;
    }

    public static float GetRarityMultiplier(FishItem.Rarity rarity)
    {
        int index = (int)rarity;
        if (index < 0 || index >= RarityMultipliers.Length)
        {
            Debug.LogWarning($"No rarity multiplier defined for {rarity}, defaulting to 1.0x");
            return 1.0f;
        }
        return RarityMultipliers[index];
    }

    public static int CalculateSellValue(int baseValue, FishItem.Rarity rarity, FishItem.FishSize size)
    {
        float rarityMultiplier = GetRarityMultiplier(rarity);

        int numSizes = System.Enum.GetValues(typeof(FishItem.FishSize)).Length;
        int sizeIndex = (int)size;
        float sizeMidpoint = (numSizes - 1) / 2f;
        float sizeAdjustmentFactor = 0.25f;

        float sizeBonusFactor = Mathf.Abs(sizeIndex - sizeMidpoint) * sizeAdjustmentFactor;

        return Mathf.FloorToInt(baseValue * rarityMultiplier * (1 + sizeBonusFactor));
    }

    public static float CalculateSizeGivenForcedSize(FishItem species, FishItem.FishSize size)
    {
        return CalculateFishSize(species.GetSizeLowerBound(size), species.GetSizeUpperBound(size));
    }
}
