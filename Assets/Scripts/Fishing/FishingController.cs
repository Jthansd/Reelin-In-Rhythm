using System.Collections;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    [SerializeField] Fisherman fisherman;
    [SerializeField] ReelWheel reelWheel;

    public Transform poleTip;
    public GameObject fishingLine;
    public GameObject bobber;

    public float castForce = 10f;

    private GameObject activeBobber;
    private GameObject activeLine;

    private bool bobberInWater = false;         
    private bool fishingRoutineRunning = false;  
    public bool hooked = false;

    private float checkInterval = 2f;
    private int luck = 1;

    [SerializeField] Fish fish;

    [SerializeField] MusicController musicController;

    [SerializeField] Inventory playerInventory;

    [SerializeField] FishingReward fishingReward;

    private void Update()
    {
        UpdateLine();
    }

    private void OnEnable()
    {
        WaterController.OnBobberEnteredWater += HandleBobberEntered;
    }

    private void OnDisable()
    {
        WaterController.OnBobberEnteredWater -= HandleBobberEntered;
    }

    public void Cast()
    {
        // If already fishing, reel in / cancel
        if (bobberInWater || activeBobber != null)
        {
            ReelIn();
            return;
        }

        // Spawn bobber + line
        activeBobber = Instantiate(bobber, poleTip.position, poleTip.rotation);
        activeLine = Instantiate(fishingLine, poleTip.position, poleTip.rotation);

        // Apply force
        Rigidbody rb = activeBobber.GetComponent<Rigidbody>();
        rb.linearVelocity = poleTip.forward * castForce;
    }

    private void UpdateLine()
    {
        if (activeBobber == null || activeLine == null)
            return;

        Vector3 start = poleTip.position;
        Vector3 end = activeBobber.transform.position;

        activeLine.transform.position = (start + end) * 0.5f;
        activeLine.transform.LookAt(end);

        float dist = Vector3.Distance(start, end);
        Vector3 scale = activeLine.transform.localScale;
        scale.z = dist * 0.5f;
        activeLine.transform.localScale = scale;
    }

    private void HandleBobberEntered(GameObject bobber)
    {
        if (bobber != activeBobber)
            return;

        Debug.Log("FishingController: Bobber entered water");

        bobberInWater = true;

        if (!fishingRoutineRunning)
            StartCoroutine(FishingCoroutine());
    }

    private IEnumerator FishingCoroutine()
    {
        fishingRoutineRunning = true;

        Debug.Log("Started fishing!");

        while (bobberInWater && !hooked)
        {
            Debug.Log("Checking for fish...");
            yield return new WaitForSeconds(checkInterval);

            int roll = Random.Range(1, luck + 1);
            Debug.Log($"Rolled a {roll} (1 in {luck} chance)");

            if (roll == 1)
            {
                hooked = true;
                HandleHookedAction();
            }
        }

        fishingRoutineRunning = false;
    }

    private void HandleHookedAction()
    {
        Debug.Log("Fish hooked!");
        fisherman.StartReeling();
        StartCoroutine(ReelingRoutine());
    }

    private IEnumerator ReelingRoutine()
    {
        reelWheel.StartReelWheel();
        yield return new WaitUntil(() => reelWheel.isCaught);
        fisherman.Reel();
    }

    public void ReeledIn()
    {
        Debug.Log("Fish reeled in!");

        hooked = false;
        bobberInWater = false;

        if (activeBobber != null)
            Destroy(activeBobber);

        if (activeLine != null)
            Destroy(activeLine);

        activeBobber = null;
        activeLine = null;

        Debug.Log("You caught a fish!" + $" It was a {fish.FishName}. {fish.Description}");
        musicController.StopMusic();

        // Add the fish to the player's inventory
        playerInventory.AddItem(fishingReward.GetRandomReward());

    }

    private void ReelIn()
    {
        // Player manually reels in early
        bobberInWater = false;

        if (activeBobber != null)
            Destroy(activeBobber);

        if (activeLine != null)
            Destroy(activeLine);

        activeBobber = null;
        activeLine = null;

        hooked = false;
    }
}
