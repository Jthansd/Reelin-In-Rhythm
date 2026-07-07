using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private string itemType;
    [SerializeField] private Sprite icon;

    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public string ItemType => itemType;

    public Sprite Icon => icon;
}
