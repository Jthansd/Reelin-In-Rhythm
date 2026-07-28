using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RarityConfig", menuName = "Scriptable Objects/Rarity Config")]
public class RarityConfig : ScriptableObject
{
    public List<RarityWeightConfig> rarityWeights;

    public void AutoGenerateWeights(float peakFactor = 0.6f, float spreadFactor = 0.9f, float floorWeight = 0.02f)
    {
        rarityWeights.Sort((a, b) => a.unlockThreshold.CompareTo(b.unlockThreshold));

        for (int i = 0; i < rarityWeights.Count; i++)
        {
            float thisThreshold = rarityWeights[i].unlockThreshold;
            float nextThreshold = (i + 1 < rarityWeights.Count)
                ? rarityWeights[i + 1].unlockThreshold
                : 100f;

            float gap = nextThreshold - thisThreshold;

            rarityWeights[i].peakStrength = thisThreshold + peakFactor * gap;
            rarityWeights[i].spread = gap * spreadFactor;
            rarityWeights[i].floorWeight = floorWeight;
        }
    }

    public float GetWeight(RarityWeightConfig config, float rarityStrength)
    {
        if (rarityStrength < config.unlockThreshold)
            return 0f;

        float distance = rarityStrength - config.peakStrength;
        float weight = Mathf.Exp(-(distance * distance) / (2f * config.spread * config.spread));
        return Mathf.Max(weight, config.floorWeight);
    }
}

[System.Serializable]
public class RarityWeightConfig
{
    public FishItem.Rarity rarity;
    public float unlockThreshold;
    public float peakStrength;
    public float spread;
    public float floorWeight = 0.02f;
}