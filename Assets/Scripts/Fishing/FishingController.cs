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

    private EquipmentItem currentBait;


    [SerializeField] MusicController musicController;

    [SerializeField] Inventory playerInventory;

    [SerializeField] FishingReward fishingReward;

    [SerializeField] PlayerStats playerStats;

    [SerializeField] PlayerEquipment playerEquipment;

    [SerializeField] RarityConfig rarityConfig;

    //1. apply bait bonus when player casts
    //2. Remove bait bonus if player cancels fishing
    //3. consume the bait when the player hooks a fish
    //4. Remove the bait bonus when the fishing event is over (player win or player loss)
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
        //1. Apply bait bonus when player casts

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

        ApplyBait();
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
                //hooked = true;
                HandleHookedAction();
            }
        }

        fishingRoutineRunning = false;
    }

    private void HandleHookedAction()
    {
        Debug.Log("Fish hooked!");
        hooked = true;
        if (playerEquipment.ConsumeBait(out EquipmentItem baitUsed))
        {
            currentBait = baitUsed;
        }
        else
        {
            currentBait = null; // No bait was used
            Debug.Log("No bait equipped.");
        }

        //get the fish that was hooked
        if (fishingReward.GetRandomFishWithRarity(DetermineRarity()) is FishItem fish)
        {
            currentFish = fish;
        }
        
        fisherman.StartReeling();
        StartCoroutine(ReelingRoutine());
    }
    

    //TODO: Edit this to only apply and consume the bait if a fish is hooked
    //if the player reels in early we do not consume the bait
    //also if the player hooks a fish but loses it, bait is consumed
    private void ApplyBait()
    {
        playerEquipment.ApplyBaitBonus();

        //if (playerEquipment.ConsumeBait(out EquipmentItem baitUsed))
        //{
        //    Debug.Log("Bait applied to the hooked fish!");
        //    // Implement any additional logic for bait effects here
        //    Debug.Log($"Bait used: {baitUsed.name}");

        //    currentBait = baitUsed; // Store the bait used for later reference
        //}
        //else
        //{
        //    currentBait = null; // No bait was used
        //    Debug.Log("No bait equipped.");
        //}
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
        
        playerInventory.AddItem(currentFish);
        playerEquipment.RevertBaitBuff(currentBait); // Revert the bait bonus after the fishing event is over


    }

    private void ReelIn()
    {
        if (hooked)
        {
            return; // Don't allow reeling in if a fish is hooked
        }
        // Player manually reels in early
        bobberInWater = false;

        if (activeBobber != null)
            Destroy(activeBobber);

        if (activeLine != null)
            Destroy(activeLine);

        activeBobber = null;
        activeLine = null;

        hooked = false;
        playerEquipment.RevertBaitBuff(); // Revert the bait bonus if the player cancels fishing
    }
}
