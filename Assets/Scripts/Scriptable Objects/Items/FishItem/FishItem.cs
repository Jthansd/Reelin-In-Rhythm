using UnityEngine;

[CreateAssetMenu(fileName = "FishItem", menuName = "Scriptable Objects/FishItem")]
public class FishItem : Item
{
    [SerializeField] private int sellValue;
    [SerializeField] private string rarity;

    public int SellValue => sellValue;
    public string Rarity => rarity;
}
