using UnityEditor;
using UnityEngine;

public static class FishingMath
{

    private const float BaselineCatchStrength = 10f;
    private const float FishScalingFactor = 1.75f;
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
}
