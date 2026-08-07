using UnityEngine;

[CreateAssetMenu(fileName = "FishItem", menuName = "Scriptable Objects/FishItem")]
public class FishItem : Item
{
    public enum Rarity {
        Common, //0
        Uncommon, // 10
        Rare, //25
        SuperRare, //50
        Legendary, //75
        Impossible //101
    }


    [SerializeField] private int sellValue;
    [SerializeField] private Rarity rarityType;
    private float baseDifficulty = 10;
    [SerializeField] private float customDifficultyMultiplier = 1.0f;
    public int SellValue => sellValue;
    public Rarity rarity => rarityType;
    public float BaseDifficulty => baseDifficulty;
    public float CustomDifficultyMultiplier => customDifficultyMultiplier;

    
    public int GetRarityMultiplier()
    {
        switch (rarityType)
        {
            case Rarity.Common:
                return 0;
            case Rarity.Uncommon:
                return 1;
            case Rarity.Rare:
                return 2;
            case Rarity.SuperRare:
                return 3;
            case Rarity.Legendary:
                return 4;
            case Rarity.Impossible:
                return 5;
            default:
                return 0;
        }
    }

}
