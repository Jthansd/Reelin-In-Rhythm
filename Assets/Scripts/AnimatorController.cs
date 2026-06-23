using UnityEngine;

public class AnimatedController : MonoBehaviour
{
    [SerializeField] float moveSpeed; // useful to observe for debugging
    MovementController moveController;
    Animator animator;
    FishingController fishingController;
    protected Animator Animator { get { return animator; } }
    void Start()
    {
        animator = GetComponent<Animator>();
        moveController = GetComponent<MovementController>();
        fishingController = FindAnyObjectByType<FishingController>();
    }

    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    void Update()
    {
        moveSpeed = moveController.GetHorizontalSpeedPercent();
        animator.SetFloat("Speed", moveSpeed);
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public bool GetIsFishing()
    {
        return animator.GetBool("IsFishing");
    }


}
