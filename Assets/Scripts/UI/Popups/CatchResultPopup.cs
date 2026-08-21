using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatchResultPopup : MonoBehaviour, IPopup
{
    [SerializeField] private Image fishIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private Image cardBackground;
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float displayDuration = 2f;
    

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void ForceDismiss()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void Show(CaughtFish fish)
    {
        if (fish != null)
        {
            fishIcon.sprite = fish.species.Icon;
            nameText.text = fish.species.ItemName;
            rarityText.text = fish.rarity.ToString();
            cardBackground.color = GetRarityColor(fish.rarity);
        }

        StartCoroutine(PopAndDismiss());
    }

    private Color GetRarityColor(FishItem.Rarity rarity)
    {
        return FishItem.RarityColor(rarity);
    }

    private System.Collections.IEnumerator PopAndDismiss()
    {
        float t = 0f;
        rect.localScale = Vector3.zero;

        while (t < popDuration)
        {
            t += Time.deltaTime;
            float scale = Mathf.SmoothStep(0f, 1f, t / popDuration);
            rect.localScale = Vector3.one * scale;
            yield return null;
        }
        rect.localScale = Vector3.one;

        yield return new WaitForSeconds(displayDuration);

        Destroy(gameObject);
    }
}