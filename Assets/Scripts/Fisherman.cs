using UnityEngine;

public class Fisherman : MonoBehaviour
{
    AnimatedController animator;
    [SerializeField] FishingController fishingController;


    private void Awake()
    {
        animator = GetComponent<AnimatedController>();
        Debug.Assert(animator != null, "AnimatedController component is missing from the GameObject.");
        fishingController = FindAnyObjectByType<FishingController>();
        Debug.Assert(fishingController != null, "FishingController not found in scene.");

    }
    public void Cast()
    {
        if(animator.GetComponent<Animator>().GetBool("isFishing"))
        {
            Debug.Log("Reeling in the fishing line...");
            animator.SetBool("isFishing", false);
            fishingController.Cast();
            return;
        }
        Debug.Log("Casting the fishing line...");
        animator.SetTrigger("Cast");

    }

    public void OnCastRelease()
    {
        Debug.Log("Cast animation completed, now casting the line.");
        fishingController.Cast();
        animator.SetBool("isFishing", true);
    }

    public void Reel()
    {
        animator.SetBool("isFishing", false);
        animator.SetBool("isReeling", false);
        fishingController.ReeledIn();
    }

    public void StartReeling()
    {
        animator.SetBool("isReeling", true);
    }
}
