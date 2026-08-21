[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int itemQuantity;

    // Never Inspector-authored - only ever constructed at runtime by the fishing flow.
    // NonSerialized so Unity can't materialize a default (all-null) instance for slots
    // configured in the editor, which would make IsCaughtFish wrongly true for them.
    [System.NonSerialized] public CaughtFish caughtFish;
    public bool IsCaughtFish => caughtFish != null;
}