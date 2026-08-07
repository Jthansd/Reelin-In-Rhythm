using UnityEngine;

public class FishMeter : MonoBehaviour
{
    [SerializeField] private ProgressBar FishingProgress;

    public float advanceSpeed = 0.1f;
    public float decaySpeed = 0.1f;
    private float startingProgress = 0.5f;
    


    public int Advance(bool hit, float progress)
    {
        if (hit)
        {
            return FishingProgress.UpdateProgress(-progress / 2);
        }
        else
        {
            return FishingProgress.UpdateProgress(progress/2);
        }
        
    }

   
    public void ResetProgress()
    {
        FishingProgress.SetProgress(startingProgress);
    }
}
