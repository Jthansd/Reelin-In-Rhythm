using UnityEngine;

public class StatBoostDebugTester : MonoBehaviour
{
    [SerializeField] private StatBoostManager statBoostManager;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private StatBoost boostToTest;

    [ContextMenu("Test Purchase")]
    public void TestPurchase()
    {
        int levelBefore = statBoostManager.GetPurchaseCount(boostToTest);
        int cost = statBoostManager.GetNextCost(boostToTest);

        Debug.Log($"Attempting purchase: {boostToTest.title} | level {levelBefore} -> {levelBefore + 1} | cost {cost}");
        Debug.Log($"Before: Luck={playerStats.Luck}, CatchStrength={playerStats.CatchStrength}, RarityStrength={playerStats.RarityStrength}");

        bool success = statBoostManager.TryPurchase(boostToTest);

        Debug.Log($"Purchase {(success ? "SUCCEEDED" : "FAILED")}");
        Debug.Log($"After: Luck={playerStats.Luck}, CatchStrength={playerStats.CatchStrength}, RarityStrength={playerStats.RarityStrength}");
        Debug.Log($"New level: {statBoostManager.GetPurchaseCount(boostToTest)} / {boostToTest.maxPurchases}");
    }

    [ContextMenu("Test Max Purchases")]
    public void TestMaxPurchases()
    {
        for (int i = 0; i < boostToTest.maxPurchases + 2; i++) // deliberately try 2 past the cap
        {
            bool success = statBoostManager.TryPurchase(boostToTest);
            Debug.Log($"Purchase attempt {i + 1}: {(success ? "SUCCEEDED" : "FAILED")} | level now {statBoostManager.GetPurchaseCount(boostToTest)}");
        }
    }
}