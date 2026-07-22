using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private Sprite icon;
    [SerializeField] private string itemID;

    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public Sprite Icon => icon;
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
