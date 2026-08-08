using UnityEngine;

public static class FishingMath
{
    public static float CalculateHitProgress(float playerCatchStrength, float fishDifficulty, float hitsNeededAtParity)
    {
        return (playerCatchStrength / fishDifficulty) / hitsNeededAtParity;
    }

    public static float CalculateFishDifficultyProduct(float baseDifficulty, int fishRarityFactor, float perFishMultiplier)
    {
        return baseDifficulty * Mathf.Pow(1.25f, fishRarityFactor) * perFishMultiplier;
    }

    public static float CalculateNotesNeededAtParity(float percentagePassing, int noteCount)
    {
        return noteCount * percentagePassing;
    }
}
