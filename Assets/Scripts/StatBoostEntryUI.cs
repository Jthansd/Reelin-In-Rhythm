using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class StatBoostEntryUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image[] segments; // assign all 10 in order, index 0-9
    [SerializeField] private Color filledColor = Color.yellow;
    [SerializeField] private Color emptyColor = Color.gray;

    private Action onPurchase;

    public void Bind(Sprite iconSprite, string price, int currentLevel, int maxLevel, Action purchaseAction)
    {
        icon.sprite = iconSprite;
        priceText.text = price;
        onPurchase = purchaseAction;

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].color = i < currentLevel ? filledColor : emptyColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPurchase?.Invoke();
    }
}