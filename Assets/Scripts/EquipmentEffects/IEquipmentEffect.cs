public interface IEquipmentEffect
{
    // Fires once when a fish is hooked, before rarity/species is finalized.
    void OnBeforeFishSelected(FishSelectionContext context) { }

    // Fires on every missed note during reeling.
    // Returns the (possibly modified) penalty to apply - unchanged value means no effect.
    float OnNoteMissed(FishingController context, float currentPenalty) { return currentPenalty; }

    // Fires once when a fish is successfully caught, before it's added to inventory.
    void OnCatchSuccess(CaughtFish caughtFish) { }
}