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
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI hitRatioText;
    [SerializeField] private ProgressBar hitRatioBar;


    private RectTransform rect;
    private Vector3 targetScale; // the scale authored on the prefab root - the pop animation grows to this

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        targetScale = rect.localScale;
    }

    public void ForceDismiss()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void Show(CaughtFish fish, float hitRatio = 0f)
    {
        if (fish != null)
        {
            fishIcon.sprite = fish.species.Icon;
            nameText.text = fish.species.ItemName;
            rarityText.text = fish.rarity.ToString();
            sizeText.text = $"Size: {fish.fishSize}";
            valueText.text = $"Value: {fish.sellPrice}";
            hitRatioText.text = $"Notes Hit: {hitRatio * 100f}%";
            hitRatioBar.SetProgress(hitRatio);
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
            rect.localScale = targetScale * scale;
            yield return null;
        }
        rect.localScale = targetScale;

        yield return new WaitForSeconds(displayDuration);

        Destroy(gameObject);
    }
}