using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private string itemType;
    [SerializeField] private Sprite icon;
    [SerializeField] private string itemID;
    [SerializeField] private int itemValue;

    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public string ItemType => itemType;
    public Sprite Icon => icon;
    public int ItemValue => itemValue;
    public string ItemID => itemID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
