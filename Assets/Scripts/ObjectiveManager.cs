using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [SerializeField] private List<Objective> availableObjectives;
    [SerializeField] private FishingController fishingController;
    [SerializeField] private ItemDatabase fishDatabase;
    [SerializeField] private GameObject objectivePopupPrefab;
    [SerializeField] private RectTransform canvasTransform;

    private class ObjectiveRuntimeProgress
    {
        public int simpleCount = 0;
        public HashSet<FishItem.Rarity> rarityHitSet = new();
    }

    private Dictionary<string, ObjectiveRuntimeProgress> progress = new();
    private HashSet<string> completedObjectives = new();

    // Fired when a quest completes, so UI (popup, quest log) can react.
    public event Action<Objective> OnObjectiveCompleted;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        fishingController.OnFishCaught += HandleFishCaught;
        OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        fishingController.OnFishCaught -= HandleFishCaught;
        OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private List<Objective> GetActiveObjectives()
    {
        return availableObjectives.Where(o => !completedObjectives.Contains(o.objectiveId)).ToList();
    }

    private ObjectiveRuntimeProgress GetOrCreateProgress(string objectiveId)
    {
        if (!progress.TryGetValue(objectiveId, out var o))
        {
            o = new ObjectiveRuntimeProgress();
            progress[objectiveId] = o;
        }
        return o;
    }

    private void HandleFishCaught(string fishId)
    {
        FishItem fish = fishDatabase.GetItemByID(fishId) as FishItem;
        if (fish == null) return;

        foreach (var objective in GetActiveObjectives())
        {
            var o = GetOrCreateProgress(objective.objectiveId);

            switch (objective.objectiveType)
            {
                case ObjectiveType.CatchCount:
                    o.simpleCount++;
                    CheckComplete(objective, o.simpleCount >= objective.targetCount);
                    break;

                case ObjectiveType.CatchRarityCount:
                    if (fish.rarity == objective.requiredRarity)
                    {
                        o.simpleCount++;
                        CheckComplete(objective, o.simpleCount >= objective.targetCount);
                    }
                    break;

                case ObjectiveType.CatchOneOfEachRarity:
                    o.rarityHitSet.Add(fish.rarity);
                    int totalRarities = FishItem.GetPlayerFacingRarities().Count;
                    CheckComplete(objective, o.rarityHitSet.Count >= totalRarities);
                    break;

                case ObjectiveType.CatchAllOfRarity:
                    bool allCaught = fishDatabase.GetAllItemsOfRarity(objective.requiredRarity)
                        .All(item => FishOPedia.Instance.IsDiscovered(item.ItemID));
                    CheckComplete(objective, allCaught);
                    break;
            }
        }
    }

    private void CheckComplete(Objective objective, bool isComplete)
    {
        if (!isComplete) return;
        if (completedObjectives.Contains(objective.objectiveId)) return;

        completedObjectives.Add(objective.objectiveId);

        if (objective.currencyReward > 0)
        {
            CurrencyManager.Instance.AwardCurrency(objective.currencyReward);
        }

        Debug.Log($"Objective completed: {objective.title}");
        OnObjectiveCompleted?.Invoke(objective);
    }

    private void HandleObjectiveCompleted(Objective objective)
    {
        if (objective != null && objectivePopupPrefab != null)
        {
            GameObject popup = Instantiate(objectivePopupPrefab, canvasTransform);
            ObjectivePopup objectivePopup = popup.GetComponent<ObjectivePopup>();

            if (objectivePopup != null)
            {
                objectivePopup.Show(objective);
            }
        }
    }
    public bool IsCompleted(string objectiveId) => completedObjectives.Contains(objectiveId);
}