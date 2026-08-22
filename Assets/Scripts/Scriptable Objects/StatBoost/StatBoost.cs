using UnityEngine;

public enum StatBoostType
{
    Luck,
    CatchStrength,
    RarityStrength
}

[CreateAssetMenu(fileName = "StatBoost", menuName = "Scriptable Objects/StatBoost")]
public class StatBoost : ScriptableObject
{
    public string boostId;
    public string title;
    public string description;
    public StatBoostType statType;

    public int maxPurchases = 5;

    // For Luck: flat amount added per purchase.
    // For CatchStrength/RarityStrength: multiplier bonus per purchase, e.g. 0.1 = +10% per level.
    public float perPurchaseAmount;

    public int baseCost = 100;
    public float costGrowthFactor = 1.5f; // each purchase costs more than the last
}