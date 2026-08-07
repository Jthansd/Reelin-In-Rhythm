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
        SetProgress(this.progress + amount);
        if(this.progress < 1f && this.progress > 0f)
        {
            return 0; // Progress bar is neither full nor empty
        }
        else if(this.progress <= 0f)
        {
            return 1; // Progress bar is empty
        }
        else
        {
            return -1; // Progress bar is full
        }
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
