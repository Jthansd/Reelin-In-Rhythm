using System.Collections;
using System.Collections.Generic;
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

    private FishItem currentFish;


    [SerializeField] MusicController musicController;

    [SerializeField] Inventory playerInventory;

    [SerializeField] FishingReward fishingReward;

    [SerializeField] PlayerStats playerStats;

    [SerializeField] PlayerEquipment playerEquipment;

    [SerializeField] RarityConfig rarityConfig;


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

            int roll = Random.Range(1, 10 - playerStats.Luck);
            Debug.Log($"Rolled a {roll} (1 in {10 - playerStats.Luck} chance)");

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
        //get the fish that was hooked
        if(fishingReward.GetRandomFishWithRarity(DetermineRarity()) is FishItem fish)
        {
            currentFish = fish;
        }
        fisherman.StartReeling();
        StartCoroutine(ReelingRoutine());
    }


    public FishItem.Rarity DetermineRarity()
    {
        Debug.Log("Determine Rarity was called");
        int strength = playerStats.RarityStrength; //player rarityStrength

        List<(FishItem.Rarity rarity, float weight)> weightedPool = new(); //empty list of rarities and they're weights
        float totalWeight = 0f;

        foreach (var config in rarityConfig.rarityWeights) //for each weight config in rarityWeightsConfig
        {
            float weight = rarityConfig.GetWeight(config, strength); //get the weight of a rarity
            if (weight <= 0f) continue; // still locked, skip entirely

            weightedPool.Add((config.rarity, weight)); //add that weight to the pool
            totalWeight += weight; //add to total weight
        }

        float roll = Random.Range(0f, totalWeight); //pick a random value from the total cumulative weight
        float cumulative = 0f;

        foreach (var (rarity, weight) in weightedPool)//for each rarity + weight in the pool of rarity weights
        {
            cumulative += weight; //update the cumulative weigth with the rarity weigth
            if (roll <= cumulative) //if the roll was within the cumulative amount
            {
                Debug.Log("The rarity roll determine that the fish is " + rarity.ToString());
                return rarity; // then  return the rarity the roll was within the weight range of
            }
        }


        Debug.Log("Fish was common cause something went wrong");
        return FishItem.Rarity.Common; // fallback, should only hit this on floating-point edge cases
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

        musicController.StopMusic();

        // Add the fish to the player's inventory
        //TODO: Do not award random fish, get the fish when the player hooks the fish, then award later
        playerInventory.AddItem(currentFish);

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
