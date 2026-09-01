using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipmentShopUI : ShopTabBase<EquipmentItem>
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private GameObject toolTipPrefab;
    [SerializeField] private RectTransform canvasTransform;
    private GameObject toolTip;
    private Coroutine spawnRoutine;

    public event Action<string> OnEquipmentPurchase;

    [ContextMenu("Test Refresh")]
    public void TestRefresh()
    {
        Refresh();
    }

    protected override List<(string groupName, List<EquipmentItem> items)> GetGroupedItems()
    {
        var result = new List<(string, List<EquipmentItem>)>();
        foreach (EquipmentItem.EquipmentType type in Enum.GetValues(typeof(EquipmentItem.EquipmentType)))
        {
            List<EquipmentItem> itemsOfType = new();
            foreach (var item in itemDatabase.GetAllItems())
            {
                if (item is EquipmentItem eq && eq.getEquipmentType == type)
                {
                    itemsOfType.Add(eq);
                }
            }
            if (itemsOfType.Count > 0)
                result.Add((type.ToString() + "s", itemsOfType));
        }
        return result;
    }

    private bool IsUnlocked(EquipmentItem item)
    {
        IUnlockCondition condition = item.CreateUnlockRequirement(playerEquipment);
        return condition == null || condition.IsUnlocked();
    }


    private void SpawnTooltip(string name, string description, bool canAfford, bool locked)
    {
        CloseTooltip();
        if (string.IsNullOrEmpty(description)) return;
        if (string.IsNullOrEmpty(name)) return;

        toolTip = Instantiate(toolTipPrefab, canvasTransform);
        toolTip.SetActive(false); // stay invisible until Update() has had a chance to position it

        ShopTooltipUI tooltipUI = toolTip.GetComponent<ShopTooltipUI>();
        tooltipUI.Initialize(canvasTransform);

        TextMeshProUGUI descText = toolTip.transform.Find("DescriptionText").GetComponent<TMPro.TextMeshProUGUI>();
        TextMeshProUGUI nameText = toolTip.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        Transform affordabilityObj = toolTip.transform.Find("AffordabilityText");
        TextMeshProUGUI affordabilityText = affordabilityObj.GetComponent<TMPro.TextMeshProUGUI>();

        if (locked)
        {
            descText.text = "???";
            nameText.text = "???";
            affordabilityObj.gameObject.SetActive(false);
        }
        else
        {
            descText.text = description;
            nameText.text = name;
            affordabilityObj.gameObject.SetActive(!canAfford);
            if (!canAfford)
            {
                affordabilityObj.GetComponent<TMPro.TextMeshProUGUI>().text = "Can't afford this";
            }
        }

        spawnRoutine = StartCoroutine(ShowNextFrame(toolTip));
    }

    // Waits one frame with the tooltip still inactive (and thus not yet drawing/positioned),
    // then positions and reveals it - guarantees the first VISIBLE frame is already correctly
    // placed, instead of racing Update() and the hover event's position against each other.
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

    protected override void BindEntry(GameObject entry, EquipmentItem item)
    {
        bool unlocked = IsUnlocked(item);
        ShopEntryUI entryUI = entry.GetComponent<ShopEntryUI>();

        entryUI.Bind(item.Icon, item.Value.ToString(), item.ItemDescription, unlocked ? () => HandlePurchase(item) : null);
        entryUI.SetLocked(!unlocked);

        entryUI.OnHoverEnter += (desc, pos) =>
        {
            bool canAfford = CurrencyManager.Instance.HasCurrency(item.Value);
            SpawnTooltip(item.ItemName, desc, canAfford, !unlocked);
        };
        entryUI.OnHoverExit += CloseTooltip;
    }



    private void HandlePurchase(EquipmentItem item)
    {
        if (CurrencyManager.Instance.SpendCurrency(item.Value))
        {
            playerEquipment.EquipItem(item);
            OnEquipmentPurchase?.Invoke(item.ItemID);
            Debug.Log($"Purchased and equipped {item.ItemName}");
        }
        else
        {
            Debug.Log("Not enough currency.");
        }
        Refresh();
    }


}