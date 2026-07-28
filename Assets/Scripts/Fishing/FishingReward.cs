using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FishingReward : MonoBehaviour
{
    [SerializeField] private ItemDatabase fishDatabase;

    private FishItem.Rarity currentRarity;

    public Item GetRandomReward()
    {
        List<Item> possibleRewards = fishDatabase.GetAllItems();
        int randomIndex = RNG(possibleRewards.Count);
        return possibleRewards[randomIndex];
    }

    public Item GetRandomFishWithRarity(FishItem.Rarity rarity)
    {

        List<Item> possibleFish = fishDatabase.GetAllItemsOfRarity(rarity);
        int randomIndex = RNG(possibleFish.Count);
        Debug.Log("Rarity was: " + rarity.ToString() + " and the fish was: " + possibleFish[randomIndex]);
        return possibleFish[randomIndex];
    }

    private int RNG(int range)
    {
        return Random.Range(0, range);
    }
}
