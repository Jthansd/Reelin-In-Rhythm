using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private string prefix = "";
    [SerializeField] private int paddedDigits = 0; // pads the amount with leading 0s up to this many digits - 0 means no padding

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
        currencyText.text = prefix + CurrencyManager.Instance.GetCurrency().ToString().PadLeft(paddedDigits, '0');
    }
}