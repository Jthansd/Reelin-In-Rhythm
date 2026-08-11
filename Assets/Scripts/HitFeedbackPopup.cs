using UnityEngine;
using TMPro;

public class HitFeedbackPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float riseDistance = 40f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private RectTransform rect;
    private Vector2 startPos;
    private float elapsed;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Show(string message, Color color)
    {
        text.text = message;
        text.color = color;
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;

        rect.anchoredPosition = startPos + Vector2.up * (riseDistance * t);

        Color c = text.color;
        c.a = fadeCurve.Evaluate(t);
        text.color = c;

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}