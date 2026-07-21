using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FishingReward : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;


    public Item GetRandomReward()
    {
        List<Item> possibleRewards = itemDatabase.GetAllItems();
        int randomIndex = Random.Range(0, possibleRewards.Count);
        return possibleRewards[randomIndex];
    }
}
