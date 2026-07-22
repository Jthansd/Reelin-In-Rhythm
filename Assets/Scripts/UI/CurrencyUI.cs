using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;

    private void Start()
    {
        if (currencyText == null)
        {
            Debug.LogError("CurrencyUI: No TextMeshProUGUI component found on this GameObject.");
            return;
        }

        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyUI;
        UpdateCurrencyUI(); // also refresh immediately so the UI shows the correct value right away
    }

    public void UpdateCurrencyUI()
    {
        currencyText.text = CurrencyManager.Instance.GetCurrency().ToString();
    }
}