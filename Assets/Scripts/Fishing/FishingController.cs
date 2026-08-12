using System;
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

    private float checkInterval = 2f;

    private FishItem currentFish;
    private EquipmentItem currentBait;

    private float passingPercent = 0.7f; // 70% passing notes required to catch the fish
    private float missPenaltySeverity = 0.55f;

    [SerializeField] MusicController musicController;
    [SerializeField] Inventory playerInventory;
    [SerializeField] FishingReward fishingReward;
    [SerializeField] PlayerStats playerStats;
    [SerializeField] PlayerEquipment playerEquipment;
    [SerializeField] RarityConfig rarityConfig;
    [Header("Catch/Fail Feedback")]
    [SerializeField] private GameObject catchPopupPrefab;
    [SerializeField] private GameObject failPopupPrefab;
    [SerializeField] private RectTransform canvasTransform;

    public FishingState CurrentState { get; private set; } = FishingState.Idle;

    // Fired when a fishing encounter ends, either way. bool = was the fish caught.
    public event Action<bool> OnFishingEncounterEnded;
    public event Action<string> OnFishCaught;

    private void Update()
    {
        UpdateLine();
    }

    private void OnEnable()
    {
        WaterController.OnBobberEnteredWater += HandleBobberEntered;
        reelWheel.OnReelComplete += HandleReelComplete;

    }

    private void OnDisable()
    {
        WaterController.OnBobberEnteredWater -= HandleBobberEntered;
        reelWheel.OnReelComplete -= HandleReelComplete;
    }

    public void Cast()
    {
        if (CurrentState == FishingState.Idle)
        {
            StartCast();
        }
        else if (CurrentState == FishingState.WaitingForBite)
        {
            CancelCast();
        }
        // Hooked / Reeling: casting input is ignored entirely - Fisherman shouldn't even call
        // Cast() in these states, but this guards against it regardless.
    }

    private void StartCast()
    {
        activeBobber = Instantiate(bobber, poleTip.position, poleTip.rotation);
        activeLine = Instantiate(fishingLine, poleTip.position, poleTip.rotation);

        Rigidbody rb = activeBobber.GetComponent<Rigidbody>();
        rb.linearVelocity = poleTip.forward * castForce;

        ApplyBait();

        CurrentState = FishingState.WaitingForBite;
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

    private void HandleBobberEntered(GameObject bobberObj)
    {
        if (bobberObj != activeBobber)
            return;

        if (CurrentState != FishingState.WaitingForBite)
            return;

        Debug.Log("FishingController: Bobber entered water");
        StartCoroutine(BiteCheckCoroutine());
    }

    private IEnumerator BiteCheckCoroutine()
    {
        Debug.Log("Started fishing!");

        while (CurrentState == FishingState.WaitingForBite)
        {
            yield return new WaitForSeconds(checkInterval);

            if (CurrentState != FishingState.WaitingForBite)
                yield break; // player cancelled while we were waiting

            int roll = UnityEngine.Random.Range(1, 10 - playerStats.Luck);
            Debug.Log($"Rolled a {roll} (1 in {10 - playerStats.Luck} chance)");

            if (roll == 1)
            {
                TutorialManager.Instance.ShowIfUnseen("Hooked", "Looks like you hooked your first fish! Prepare to reel it in.");
                HandleHookedAction();
                yield break;
            }
        }
    }

    private void HandleHookedAction()
    {
        Debug.Log("Fish hooked!");
        CurrentState = FishingState.Hooked;

        if (playerEquipment.ConsumeBait(out EquipmentItem baitUsed))
        {
            currentBait = baitUsed;
        }
        else
        {
            currentBait = null;
            Debug.Log("No bait equipped.");
        }

        if (fishingReward.GetRandomFishWithRarity(DetermineRarity()) is FishItem fish)
        {
            currentFish = fish;
            DetermineDifficulty(currentFish);
        }

        

        fisherman.StartReeling();
        CurrentState = FishingState.Reeling;
        reelWheel.StartReelWheel();
    }

    private void HandleReelComplete(bool caught)
    {
        Debug.Log(caught ? "Player caught the fish!" : "Player failed to catch the fish.");

        fisherman.Reel();

        if (caught)
        {
            ReeledIn();
        }
        else
        {
            CleanUpTackle();
            playerEquipment.RevertBaitBuff(currentBait);
            SpawnFailPopup();
        }

        currentFish = null;
        currentBait = null;
        CurrentState = FishingState.Idle;

        OnFishingEncounterEnded?.Invoke(caught);
    }

    private void CancelCast()
    {
        Debug.Log("Reeling In (cancelled - no bite yet)!");
        CleanUpTackle();
        playerEquipment.RevertBaitBuff();
        CurrentState = FishingState.Idle;
    }

    private void CleanUpTackle()
    {
        if (activeBobber != null)
            Destroy(activeBobber);

        if (activeLine != null)
            Destroy(activeLine);

        activeBobber = null;
        activeLine = null;
    }

    private void DetermineDifficulty(FishItem fish)
    {
        float fishDifficulty = FishingMath.CalculateFishDifficultyProduct(
            fish.BaseDifficulty,
            fish.GetRarityMultiplier(),
            fish.CustomDifficultyMultiplier
        );

        float relativeDifficulty = fishDifficulty / playerStats.CatchStrength;

        TimingDifficulty tier;
        if (relativeDifficulty >= 1.75f)
            tier = TimingDifficulty.Hard;
        else if (relativeDifficulty > 1.0f)
            tier = TimingDifficulty.Medium;
        else
            tier = TimingDifficulty.Easy;

        musicController.SetDifficulty(tier);

        Debug.Log($"Fish difficulty: {fishDifficulty}, relative: {relativeDifficulty}, tier chosen: {tier}");
    }

    private void ApplyBait()
    {
        playerEquipment.ApplyBaitBonus();
    }

    public FishItem.Rarity DetermineRarity()
    {
        Debug.Log("Determine Rarity was called");
        float strength = playerStats.RarityStrength;

        List<(FishItem.Rarity rarity, float weight)> weightedPool = new();
        float totalWeight = 0f;

        foreach (var config in rarityConfig.rarityWeights)
        {
            float weight = rarityConfig.GetWeight(config, strength);
            if (weight <= 0f) continue;

            weightedPool.Add((config.rarity, weight));
            totalWeight += weight;
        }

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var (rarity, weight) in weightedPool)
        {
            cumulative += weight;
            if (roll <= cumulative)
            {
                Debug.Log("The rarity roll determine that the fish is " + rarity.ToString());
                return rarity;
            }
        }

        Debug.Log("Fish was common cause something went wrong");
        return FishItem.Rarity.Common;
    }

    private void ReeledIn()
    {
        Debug.Log("Fish reeled in!");

        CleanUpTackle();
        musicController.StopMusic();

        Debug.Log($"Adding {currentFish.name} to inventory.");
        playerInventory.AddItem(currentFish);
        playerEquipment.RevertBaitBuff(currentBait);
        OnFishCaught?.Invoke(currentFish.ItemID);
        TutorialManager.Instance.ShowIfUnseen("First_Catch", "You caught your first fish! Check it out by pressing [E] to open your backpack.");
        SpawnCatchPopup(currentFish);
    }

    public float GetProgress()
    {
        return FishingMath.CalculateHitProgress(
            playerStats.CatchStrength,
            FishingMath.CalculateFishDifficultyProduct(currentFish.BaseDifficulty, currentFish.GetRarityMultiplier(), currentFish.CustomDifficultyMultiplier),
            FishingMath.CalculateNotesNeededAtParity(passingPercent, musicController.GetNoteCount())
        );
    }

    public float GetMissPenalty()
    {
        return FishingMath.CalculateMissPenalty(currentFish.BaseDifficulty, FishingMath.CalculateNotesNeededAtParity(passingPercent, musicController.GetNoteCount()), missPenaltySeverity);
    }

    private void SpawnCatchPopup(FishItem fish)
    {
        if (catchPopupPrefab == null || canvasTransform == null) return;

        GameObject popup = Instantiate(catchPopupPrefab, canvasTransform);
        CatchResultPopup popupScript = popup.GetComponent<CatchResultPopup>();
        if (popupScript != null)
        {
            popupScript.Show(fish);
        }
    }

    private void SpawnFailPopup()
    {
        if (failPopupPrefab == null || canvasTransform == null) return;

        GameObject popup = Instantiate(failPopupPrefab, canvasTransform);
        CatchResultPopup popupScript = popup.GetComponent<CatchResultPopup>();
        if (popupScript != null)
        {
            popupScript.Show(null);
        }
            // FailResultPopup (or however you build it) can self-manage its own display/dismiss timing,
            // same self-contained pattern as HitFeedbackPopup - no data needed, just instantiate and let it run.
        }
    }