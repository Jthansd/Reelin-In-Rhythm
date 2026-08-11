using UnityEngine;

public class FishOPediaManager : MonoBehaviour
{
    [SerializeField] GameObject fishOPedia;
    [SerializeField] FishOPediaInventoryUI leftPage;
    [SerializeField] FishOPediaInventoryUI rightPage;
    [SerializeField] Inventory FishOPediaInventory;

    private FishItem.Rarity leftRarity = FishItem.Rarity.Common;
    private FishItem.Rarity rightRarity = FishItem.Rarity.Uncommon;

    private static readonly int minValue = (int)FishItem.Rarity.Common;
    private static readonly int maxValue = (int)FishItem.Rarity.Legendary; // stop before Impossible - not player-facing

    private void Start()
    {
        SetLeftPage(leftRarity);
        SetRightPage(rightRarity);
    }

    public void NextRarity()
    {
        int leftNext = (int)leftRarity + 2;

        if (leftNext > maxValue) return; // left page would go too far - do nothing, already at the end

        leftRarity = (FishItem.Rarity)leftNext;
        SetLeftPage(leftRarity);

        int rightNext = (int)rightRarity + 2;
        if (rightNext > maxValue)
        {
            RemovePage(rightPage); // left page landed fine, but right has nothing to show
        }
        else
        {
            rightRarity = (FishItem.Rarity)rightNext;
            SetRightPage(rightRarity);
        }
    }

    public void PreviousRarity()
    {
        Debug.Log("Previous page called");
        int leftPrev = (int)leftRarity - 2;

        if (leftPrev < minValue) return; // already at the first page

        leftRarity = (FishItem.Rarity)leftPrev;
        SetLeftPage(leftRarity);

        rightRarity = (FishItem.Rarity)((int)leftRarity + 1);
        SetRightPage(rightRarity); // paging backward always restores a valid right page,
                                   // since leftRarity just decreased and rightRarity = left + 1
    }

    private void SetLeftPage(FishItem.Rarity rarity)
    {
        leftPage.gameObject.SetActive(true);
        leftPage.SetRarityAndRefresh(rarity);
    }

    private void SetRightPage(FishItem.Rarity rarity)
    {
        rightPage.gameObject.SetActive(true);
        rightPage.SetRarityAndRefresh(rarity);
    }

    private void RemovePage(FishOPediaInventoryUI page)
    {
        page.gameObject.SetActive(false);
    }


    public void ToggleFishOPedia()
    {
        bool willBeActive = !fishOPedia.activeSelf;
        fishOPedia.SetActive(willBeActive);
        CameraOrbit.Instance.SetLookEnabled(!willBeActive);
        MenuStateEvents.SetMenuOpen(willBeActive);
    }
}