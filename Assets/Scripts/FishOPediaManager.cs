using TMPro;
using UnityEngine;

public class FishOPediaManager : MonoBehaviour
{
    [SerializeField] GameObject fishOPedia;
    [SerializeField] FishOPediaInventoryUI pediaPage;
    [SerializeField] Inventory FishOPediaInventory;
    [SerializeField] GameObject speciesEntryPrefab;
    [SerializeField] Transform leftPageTransform;
    [SerializeField] Transform rightPageTransform;

    private FishOPediaSpeciesEntry leftEntry;
    private FishOPediaSpeciesEntry rightEntry;
    private int currentEntryLeftIndex;
    private bool entryModeActive;

    private void Start()
    {
        ShowPediaPage();
    }

    // Moves to the next pair of species entries. Only meaningful while viewing entries.
    public void NextEntry()
    {
        if (!entryModeActive) return;

        int newLeftIndex = currentEntryLeftIndex + 2;
        if (newLeftIndex >= FishOPedia.Instance.GetObtainableFishCount()) return; // already at the last pair

        ShowEntryPair(newLeftIndex);
    }

    // Moves to the previous pair of species entries. Only meaningful while viewing entries.
    public void PreviousEntry()
    {
        if (!entryModeActive) return;

        int newLeftIndex = currentEntryLeftIndex - 2;
        if (newLeftIndex < 0) return; // already at the first pair

        ShowEntryPair(newLeftIndex);
    }

    private void ShowPediaPage()
    {
        pediaPage.gameObject.SetActive(true);
        pediaPage.Refresh();
    }

    private void RemovePage(FishOPediaInventoryUI page)
    {
        page.gameObject.SetActive(false);
    }

    public void ToggleFishOPedia()
    {
        bool willBeActive = !fishOPedia.activeSelf;

        if (!willBeActive)
        {
            ReturnToPedia(); 
        }

        fishOPedia.SetActive(willBeActive);
        CameraOrbit.Instance.SetLookEnabled(!willBeActive);
        MenuStateEvents.SetMenuOpen(willBeActive);
    }

    // Opens the book to the species entry for the clicked fish. 
    public void SwitchToEntry(FishItem species)
    {
        int index = FishOPedia.Instance.GetObtainableFishIndex(species);
        if (index < 0)
        {
            Debug.LogWarning($"FishOPediaManager: {species.ItemName} was not found in the obtainable fish list.");
            return;
        }

        int leftIndex = index % 2 == 0 ? index : index - 1;
        ShowEntryPair(leftIndex);
    }

    // Closes any open species entries and returns to the regular slot-grid pedia view.
    public void ReturnToPedia()
    {
        DestroyEntries();
        entryModeActive = false;
        ShowPediaPage();
    }

    private void ShowEntryPair(int leftIndex)
    {
        DestroyEntries();
        RemovePage(pediaPage);

        leftEntry = CreateEntryAt(leftPageTransform, leftIndex);
        rightEntry = CreateEntryAt(rightPageTransform, leftIndex + 1);

        currentEntryLeftIndex = leftIndex;
        entryModeActive = true;
    }

    private FishOPediaSpeciesEntry CreateEntryAt(Transform parent, int fishIndex)
    {
        FishItem species = FishOPedia.Instance.GetObtainableFishAt(fishIndex);
        if (species == null) return null; 

        GameObject entryGO = Instantiate(speciesEntryPrefab, parent);
        RectTransform rt = entryGO.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        entryGO.SetActive(false);
        FishOPediaSpeciesEntry entryComponent = entryGO.GetComponent<FishOPediaSpeciesEntry>();

        string id = species.ItemID;
        bool discovered = FishOPedia.Instance.IsDiscovered(species);
        float biggest = discovered ? FishOPedia.Instance.GetBiggestSize(id) : 0f;
        float smallest = discovered ? FishOPedia.Instance.GetSmallestSize(id) : 0f;

        entryComponent.SetEntryComponents(species, biggest, smallest, species.GetFishSizeFromSizeValue(biggest), species.GetFishSizeFromSizeValue(smallest), FishOPedia.Instance.GetAllObtainedRarityOfSpecies(id), discovered);

        entryGO.SetActive(true);
        return entryComponent;
    }

    private void DestroyEntries()
    {
        if (leftEntry != null) Destroy(leftEntry.gameObject);
        if (rightEntry != null) Destroy(rightEntry.gameObject);
        leftEntry = null;
        rightEntry = null;
    }

    public void ClearUIObject(GameObject UI)
    {
        UI.SetActive(false);
    }
}
