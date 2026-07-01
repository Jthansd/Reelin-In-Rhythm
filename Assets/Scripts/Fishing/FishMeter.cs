using UnityEngine;

public class FishMeter : MonoBehaviour
{
    [SerializeField] private ProgressBar FishingProgress;

    public float advanceSpeed = 0.1f;
    public float decaySpeed = 0.05f;


    public bool Advance()
    {
        if (FishingProgress.UpdateProgress(-advanceSpeed)){
            return true;
        }
        return false;
    }

    public void Decay()
    {
        if (FishingProgress.UpdateProgress(decaySpeed))
        {

        }
    }
    public void ResetProgress()
    {
        FishingProgress.SetProgress(0.5f);
    }
}
