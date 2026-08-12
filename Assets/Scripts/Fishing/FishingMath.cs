using UnityEngine;

public static class FishingMath
{

    private const float BaselineCatchStrength = 10f;
    private const float FishScalingFactor = 1.75f;
    public static float CalculateHitProgress(float playerCatchStrength, float fishDifficulty, float hitsNeededAtParity)
    {
        return (playerCatchStrength / fishDifficulty) / hitsNeededAtParity;
    }

    public static float CalculateFishDifficultyProduct(float baseDifficulty, int fishRarityFactor, float perFishMultiplier)
    {
        return baseDifficulty * Mathf.Pow(FishScalingFactor, fishRarityFactor) * perFishMultiplier;
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
}
