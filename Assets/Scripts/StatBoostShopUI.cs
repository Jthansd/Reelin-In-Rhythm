using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatBoostShopUI : ShopTabBase<StatBoost>
{
    [SerializeField] private List<StatBoost> availableBoosts;

    protected override bool UseGridLayout => false;
    protected override List<(string groupName, List<StatBoost> items)> GetGroupedItems()
    {
        return new List<(string, List<StatBoost>)> { ("", availableBoosts) };
    }

    protected override void BindEntry(GameObject entry, StatBoost boost)
    {
        int cost = StatBoostManager.Instance.GetNextCost(boost);
        bool maxed = StatBoostManager.Instance.IsMaxed(boost);
        int level = StatBoostManager.Instance.GetPurchaseCount(boost);

        string priceLabel = maxed ? "MAX" : cost.ToString();

        entry.GetComponent<StatBoostEntryUI>().Bind(
            boost.icon,
            priceLabel,
            level,
            boost.maxPurchases,
            () => HandlePurchase(boost)
        );
    }

    private void HandlePurchase(StatBoost boost)
    {
        if (StatBoostManager.Instance.TryPurchase(boost))
        {
            Refresh();
        }
    }
}