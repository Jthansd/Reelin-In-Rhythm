using UnityEngine;
using static FishItem;

public enum ObjectiveType
{
    CatchCount,          // catch any N fish
    CatchRarityCount,    // catch N fish of a specific rarity
    CatchOneOfEachRarity,
    CatchAllOfRarity     // every distinct fish in a given rarity - delegates to FishOPedia
}

[CreateAssetMenu(fileName = "Objectve", menuName = "Scriptable Objects/Objective")]
public class Objective : ScriptableObject
{
    public string objectiveId;
    public string title;
    public string description;
    public ObjectiveType objectiveType;
    public FishItem.Rarity requiredRarity; // used by CatchRarityCount and CatchAllOfRarity
    public int targetCount;                 // used only by CatchCount / CatchRarityCount
    public int currencyReward;
    public FishItem.Rarity objectiveRarity;


    public Color GetColor()
    {
        switch (objectiveRarity)
        {
            case Rarity.Common:
                return Color.blue;
            case Rarity.Uncommon:
                return Color.green;
            case Rarity.Rare:
                return Color.orange;
            case Rarity.SuperRare:
                return new Color(0.7f, 0.3f, 1f);
            case Rarity.Legendary:
                return new Color(1f, 0.8f, 0.1f);
            default:
                return Color.grey;
        }
    }
}