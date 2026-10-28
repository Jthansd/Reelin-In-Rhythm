using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellShopUI : ShopTabBase<CaughtFish>
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GameObject toolTipPrefab;
    [SerializeField] private RectTransform canvasTransform;
    private GameObject toolTip;
    private Coroutine spawnRoutine;

    public event Action<string> OnFishSale;

    // Rebuild whenever the sell panel is shown, so it always reflects the current inventory
    private void OnEnable()
    {
        Refresh();
    }

    private void OnDisable()
    {
        CloseTooltip();
    }

    [ContextMenu("Test Refresh")]
    public void TestRefresh()
    {
        Refresh();
    }

    protected override List<(string groupName, List<CaughtFish> items)> GetGroupedItems()
    {
        var result = new List<(string, List<CaughtFish>)>();
        if (playerInventory == null) return result;

        foreach (FishItem.Rarity rarity in FishItem.GetPlayerFacingRarities())
        {
            List<CaughtFish> fishOfRarity = new();
            foreach (var slot in playerInventory.inventorySlots)
            {
                if (slot.IsCaughtFish && slot.caughtFish.rarity == rarity)
                {
                    fishOfRarity.Add(slot.caughtFish);
                }
            }

            if (fishOfRarity.Count > 0)
            {
                fishOfRarity.Sort((a, b) =>
                {
                    int byName = string.Compare(a.species.ItemName, b.species.ItemName, StringComparison.Ordinal);
                    return byName != 0 ? byName : b.sellPrice.CompareTo(a.sellPrice);
                });
                result.Add((rarity.ToString(), fishOfRarity));
            }
        }
        return result;
    }

    private void SpawnTooltip(string name, string description)
    {
        CloseTooltip();
        if (string.IsNullOrEmpty(description)) return;
        if (string.IsNullOrEmpty(name)) return;

        toolTip = Instantiate(toolTipPrefab, canvasTransform);
        toolTip.SetActive(false); // stay invisible until ShowNextFrame has had a chance to position it

        ShopTooltipUI tooltipUI = toolTip.GetComponent<ShopTooltipUI>();
        tooltipUI.Initialize(canvasTransform);

        TextMeshProUGUI descText = toolTip.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = toolTip.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        descText.text = description;
        nameText.text = name;

        // The shared shop tooltip prefab carries an affordability line - selling never needs it
        Transform affordabilityObj = toolTip.transform.Find("AffordabilityText");
        if (affordabilityObj != null)
        {
            affordabilityObj.gameObject.SetActive(false);
        }

        spawnRoutine = StartCoroutine(ShowNextFrame(toolTip));
    }

    private IEnumerator ShowNextFrame(GameObject tooltipObject)
    {
        yield return null;

        if (tooltipObject == null) yield break;

        ShopTooltipUI tooltipUI = tooltipObject.GetComponent<ShopTooltipUI>();
        tooltipUI.UpdatePosition(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        tooltipObject.SetActive(true);

        // Force the Content Size Fitter/layout groups to resolve immediately, rather than
        // waiting for Unity's normal end-of-frame layout pass - avoids a visible one-frame
        // "wrong size, then pop to correct size" stutter right after becoming active.
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipObject.GetComponent<RectTransform>());

        // Reposition again after the rebuild, since the tooltip's own size (and therefore
        // where TransformReference ends up relative to the root) may have just changed.
        tooltipUI.UpdatePosition(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
    }

    private void CloseTooltip()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        if (toolTip == null) return;
        Destroy(toolTip);
        toolTip = null;
    }

    protected override void BindEntry(GameObject entry, CaughtFish fish)
    {
        ShopEntryUI entryUI = entry.GetComponent<ShopEntryUI>();

        string description = $"{fish.rarity} - {fish.size:F1}cm\n{fish.species.ItemDescription}";
        if (fish.perfectCatch)
        {
            description += "\nPerfect Catch!";
        }

        entryUI.Bind(fish.species.Icon, fish.sellPrice.ToString(), description, () => HandleSale(fish));

        entryUI.OnHoverEnter += (desc, pos) => SpawnTooltip(fish.species.ItemName, desc);
        entryUI.OnHoverExit += CloseTooltip;
    }

    private void HandleSale(CaughtFish fish)
    {
        playerInventory.RemoveCaughtFish(fish);
        CurrencyManager.Instance.AwardCurrency(fish.sellPrice);
        OnFishSale?.Invoke(fish.species.ItemID);
        Debug.Log($"Sold {fish.species.ItemName} for {fish.sellPrice}");

        // The clicked entry is destroyed by Refresh, so its OnHoverExit will never fire
        CloseTooltip();
        Refresh();
    }
}
