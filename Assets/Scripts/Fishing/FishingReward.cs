using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FishingReward : MonoBehaviour
{
    [SerializeField] public List<Item> possibleRewards;
    

    public Item GetRandomReward()
    {
        int randomIndex = Random.Range(0, possibleRewards.Count);
        return possibleRewards[randomIndex];
    }
}
