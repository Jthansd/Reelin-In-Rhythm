using System.Collections.Generic;
using UnityEngine;

public class StatBoostManager : MonoBehaviour
{
    public static StatBoostManager Instance { get; private set; }

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private List<StatBoost> availableBoosts;

    private Dictionary<string, int> purchaseCounts = new();

    private void Awake()
    {
        Instance = this;
    }

    public int GetPurchaseCount(StatBoost boost)
    {
        purchaseCounts.TryGetValue(boost.boostId, out int count);
        return count;
    }

    public bool IsMaxed(StatBoost boost) => GetPurchaseCount(boost) >= boost.maxPurchases;

    public int GetNextCost(StatBoost boost)
    {
        int currentLevel = GetPurchaseCount(boost);
        return Mathf.RoundToInt(boost.baseCost * Mathf.Pow(boost.costGrowthFactor, currentLevel));
    }

    public bool TryPurchase(StatBoost boost)
    {
        if (IsMaxed(boost))
        {
            Debug.Log($"{boost.title} is already at max level.");
            return false;
        }

        int cost = GetNextCost(boost);
        if (!CurrencyManager.Instance.SpendCurrency(cost))
        {
            Debug.Log("Not enough currency for stat boost.");
            return false;
        }

        int newLevel = GetPurchaseCount(boost) + 1;
        purchaseCounts[boost.boostId] = newLevel;

        ApplyBoost(boost, newLevel);
        return true;
    }

    private void ApplyBoost(StatBoost boost, int newLevel)
    {
        switch (boost.statType)
        {
            case StatBoostType.Luck:
                // Permanent, simple additive stack - just apply this purchase's increment directly.
                playerStats.AdjustLuck(Mathf.RoundToInt(boost.perPurchaseAmount));
                break;

            case StatBoostType.CatchStrength:
                // Recompute the full multiplier from total levels purchased so far, under one stable key.
                playerStats.SetCatchStrengthMultiplier(boost.boostId, 1f + (boost.perPurchaseAmount * newLevel));
                break;

            case StatBoostType.RarityStrength:
                playerStats.SetRarityMultiplier(boost.boostId, 1f + (boost.perPurchaseAmount * newLevel));
                break;
        }
    }
}