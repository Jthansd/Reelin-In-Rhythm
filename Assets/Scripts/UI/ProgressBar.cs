using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fill;
    [Range(0f, 1f)]
    public float progress = 0.5f;

    public void SetProgress(float progress)
    {
        this.progress = Mathf.Clamp01(progress);
        fill.fillAmount = this.progress;
    }

    public int UpdateProgress(float amount)
    {
        float target = this.progress + amount;
        SetProgress(target);

        if (this.progress <= 0f)
        {
            return 1; // caught - meter emptied
        }

        return 0; // still in progress (overflow above 1.0 is just clamped visually, no loss state)
    }

    void Start()
    {
        SetProgress(progress);
    }

    void OnValidate()
    {
        if (fill != null)
            fill.fillAmount = progress;
    }
}