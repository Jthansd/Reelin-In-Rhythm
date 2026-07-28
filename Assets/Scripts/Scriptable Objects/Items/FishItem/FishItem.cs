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

    public int SellValue => sellValue;
    public Rarity rarity => rarityType;

    
}
