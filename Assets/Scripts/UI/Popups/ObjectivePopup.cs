using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivePopup : MonoBehaviour, IPopup
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI titleText;
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


    public void Show(Objective objective)
    {
        if(objective != null)
        {
            background.color = objective.GetColor();
            descriptionText.text = objective.description;
            titleText.text = objective.title;
        }

        StartCoroutine(PopAndDismiss());

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
