using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI currencyText;

    private void Awake()
    {
        if (currencyText == null)
        {
            Debug.LogError("CurrencyUI: No TextMeshProUGUI component found on this GameObject.");
        }

        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyUI;
    }

    public void UpdateCurrencyUI()
    {
        currencyText.text = CurrencyManager.Instance.GetCurrency().ToString();
    }
}
