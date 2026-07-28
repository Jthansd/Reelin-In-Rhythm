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

    public bool UpdateProgress(float amount)
    {
        SetProgress(this.progress + amount);
        return this.progress <= 0f;  
    }

    void Start()
    {
        SetProgress(progress);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValidate()
    {
        if (fill != null)
            fill.fillAmount = progress;
    }

    
}
