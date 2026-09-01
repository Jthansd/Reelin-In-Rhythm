using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopEntryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMPro.TextMeshProUGUI priceText;
    [SerializeField] private Sprite lockedIcon;

    private string boundDescription;
    private Action onPurchase;

    public event Action<string, Vector2> OnHoverEnter;
    public event Action OnHoverExit;

    public void SetLocked(bool locked)
    {
        icon.sprite = locked ? lockedIcon : icon.sprite;
        priceText.text = locked ? "???" : priceText.text;

    }
    public void Bind(Sprite iconSprite, string price, string description, Action purchaseAction)
    {
        icon.sprite = iconSprite;
        priceText.text = price;
        boundDescription = description;
        onPurchase = purchaseAction;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke(boundDescription, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPurchase?.Invoke();
    }
}