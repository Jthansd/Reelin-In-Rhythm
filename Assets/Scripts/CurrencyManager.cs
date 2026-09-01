using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private int currency;

    public event Action OnCurrencyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }


    public int GetCurrency()
    {
        Debug.Log($"Current currency: {currency}");
        return currency;
    }

    public void AwardCurrency(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke();
        Debug.Log($"Awarded {amount} currency. New total: {currency}");
    }

    public bool SpendCurrency(int amount)
    {
        if (amount > currency)
        {
            Debug.LogWarning("Not enough currency to spend.");
            return false;
        }
        currency -= amount;
        OnCurrencyChanged?.Invoke();
        Debug.Log($"Spent {amount} currency. New total: {currency}");
        return true;
    }

    internal bool HasCurrency(int value)
    {
        return GetCurrency() >= value;
    }
}
