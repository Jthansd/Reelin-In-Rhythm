using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FishOPedia : MonoBehaviour
{

    [SerializeField] ItemDatabase obtainableFish;

    [SerializeField] FishingController fishingController;

    private readonly HashSet<(string speciesId, FishItem.Rarity rarity)> discoveredFishRarities = new();
    private readonly Dictionary<string, float> biggestFish = new();
    private readonly Dictionary<string, float> smallestFish = new();
    public List<Item> GetAllObtainableFish() => obtainableFish.GetAllItems();
    
    public static FishOPedia Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        fishingController.OnFishCaught += HandleFishCaught;
    }

    private void OnDisable()
    {
        fishingController.OnFishCaught -= HandleFishCaught;
    }

    private void HandleFishCaught(CaughtFish caughtFish)
    {
        NewDiscovery(caughtFish.species, caughtFish.rarity, caughtFish.size);
    }

    // Discovered at this exact rarity.
    public bool IsDiscovered(FishItem fish, FishItem.Rarity rarity)
    {
        return discoveredFishRarities.Contains((fish.ItemID, rarity));
    }

    // Discovered at any rarity - used where no specific rarity context is available.
    public bool IsDiscovered(FishItem fish)
    {
        return discoveredFishRarities.Any(d => d.speciesId == fish.ItemID);
    }

    public bool IsDiscovered(string id, FishItem.Rarity rarity)
    {
        return discoveredFishRarities.Contains((id, rarity));
    }


    public void NewDiscovery(FishItem species, FishItem.Rarity rarity, float size)
    {
        if (discoveredFishRarities.Add((species.ItemID, rarity)))
        {
            Debug.Log($"New discovery: {species.ItemName} ({rarity})");
        }
        RecordSize(species, size);
    }

    public void RecordSize(FishItem species, float size)
    {
        DetermineSizeRecord(species, size);
        DetermineSmallSizeRecord(species, size);
    }

    public void DetermineSizeRecord(FishItem species, float size)
    {
        //Search through the set of current record sizes for the given species
        if (!biggestFish.TryGetValue(species.ItemID, out float currentBiggest) || size > currentBiggest)
        {
            biggestFish[species.ItemID] = size;
        }
    }

    public void DetermineSmallSizeRecord(FishItem species, float size)
    {
        if(!smallestFish.TryGetValue(species.ItemID, out float currentSmallest) || size < currentSmallest)
        {
            smallestFish[species.ItemID] = size;
        }
    }

    public List<Item> GetDiscoveredFish()
    {
        List<Item> fishList = new List<Item>();
        foreach (var speciesId in discoveredFishRarities.Select(d => d.speciesId).Distinct())
        {
            fishList.Add(obtainableFish.GetItemByID(speciesId));
        }

        return fishList;

    }

    public float GetBiggestSize(string speciesId)
    {
        return biggestFish.TryGetValue(speciesId, out float biggest) ? biggest : 0f;
    }

    public float GetSmallestSize(string speciesId)
    {
        return smallestFish.TryGetValue(speciesId, out float smallest) ? smallest : 0f;
    }

    public int GetObtainableFishIndex(FishItem species)
    {
        return obtainableFish.GetAllItems().FindIndex(item => item.ItemID == species.ItemID);
    }

    public FishItem GetObtainableFishAt(int index)
    {
        List<Item> all = obtainableFish.GetAllItems();
        if (index < 0 || index >= all.Count) return null;
        return all[index] as FishItem;
    }

    public int GetObtainableFishCount() => obtainableFish.GetAllItems().Count;

    public List<FishItem.Rarity> GetAllObtainedRarityOfSpecies(string speciesId)
    {
        List<FishItem.Rarity> results = new List<FishItem.Rarity>();
        foreach (FishItem.Rarity rarity in Enum.GetValues(typeof(FishItem.Rarity)))
        {
            if(IsDiscovered(speciesId, rarity)){
                results.Add(rarity);
            }
        }
        return results;
    }

}