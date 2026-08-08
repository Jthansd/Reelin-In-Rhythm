using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum StatType { Luck, CatchStrength, RarityStrength }

    [SerializeField] private int luck = 1;
    [SerializeField] private float baseCatchStrength = 10f;
    [SerializeField] private float baseRarityStrength = 1f;

    // Keyed by an arbitrary source id (equipment slot name, buff id, etc.)
    // so any future permanent buff can register its own multiplier without touching this class.
    private Dictionary<string, float> catchStrengthMultipliers = new();
    private Dictionary<string, float> rarityMultipliers = new();

    public int Luck => luck;
    public float CatchStrength => baseCatchStrength * Product(catchStrengthMultipliers);
    public float RarityStrength => baseRarityStrength * Product(rarityMultipliers);

    private float Product(Dictionary<string, float> mods)
    {
        float result = 1f;
        foreach (var value in mods.Values)
            result *= value;
        return result;
    }

    public void AdjustLuck(int bonusStrength)
    {
        Debug.Log("Luck before adjustment: " + luck);
        luck += bonusStrength;
        Debug.Log("Luck after adjustment: " + luck);
    }

    public void SetCatchStrengthMultiplier(string sourceId, float multiplier)
    {
        catchStrengthMultipliers[sourceId] = multiplier;
    }

    public void ClearCatchStrengthMultiplier(string sourceId)
    {
        catchStrengthMultipliers.Remove(sourceId);
    }

    public void SetRarityMultiplier(string sourceId, float multiplier)
    {
        rarityMultipliers[sourceId] = multiplier;
    }

    public void ClearRarityMultiplier(string sourceId)
    {
        rarityMultipliers.Remove(sourceId);
    }
}