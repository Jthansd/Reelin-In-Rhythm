using System.Collections.Generic;
using UnityEngine;

public class FishOPedia : MonoBehaviour
{

    [SerializeField] ItemDatabase obtainableFish;


   

    [SerializeField] FishingController fishingController;

    private HashSet<string> discoveredFishIds = new();

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

    private void HandleFishCaught(string id)
    {
        NewDiscovery(id);
    }


    public bool IsDiscovered(FishItem fish)
    {
        return discoveredFishIds.Contains(fish.ItemID);
    }

    
    public void NewDiscovery(FishItem fish)
    {
        if (IsDiscovered(fish))
        {
            return;
        }
        else
        {
            discoveredFishIds.Add(fish.ItemID);
        }

    }

    public void NewDiscovery(string id)
    {
        if (IsDiscovered(id))
        {
            Debug.Log("Not a new fish");
            return;
        }
        else
        {
            discoveredFishIds.Add(id);
            Debug.Log("New Fish discovered");
        }
    }

    public bool IsDiscovered(string id)
    {
        return discoveredFishIds.Contains(id);
    }

    public List<Item> GetDiscoveredFish()
    {
        List<Item> fishList = new List<Item>();
        foreach (var fishId in discoveredFishIds)
        {
            fishList.Add(obtainableFish.GetItemByID(fishId));
        }

        return fishList;

    }

}