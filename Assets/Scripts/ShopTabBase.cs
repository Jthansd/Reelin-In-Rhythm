using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ShopTabBase<TItem> : MonoBehaviour
{
    [SerializeField] protected Transform contentParent;
    [SerializeField] protected GameObject sectionHeaderPrefab;
    [SerializeField] protected GameObject gridContainerPrefab;
    [SerializeField] protected GameObject shopEntryPrefab;

    // In ShopTabBase<TItem>
    protected virtual bool UseGridLayout => true;

    public virtual void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var group in GetGroupedItems())
        {
            if (!string.IsNullOrEmpty(group.groupName))
            {
                GameObject header = Instantiate(sectionHeaderPrefab, contentParent);
                header.GetComponentInChildren<TextMeshProUGUI>().text = group.groupName;
            }

            Transform itemParent = contentParent;
            if (UseGridLayout)
            {
                GameObject grid = Instantiate(gridContainerPrefab, contentParent);
                itemParent = grid.transform;
            }

            foreach (var item in group.items)
            {
                GameObject entry = Instantiate(shopEntryPrefab, itemParent);
                BindEntry(entry, item);
            }
        }
    }

 

    protected abstract List<(string groupName, List<TItem> items)> GetGroupedItems();
    protected abstract void BindEntry(GameObject entry, TItem item);
}