using UnityEngine;

public class HookRarityEffect : IEquipmentEffect
{
    private readonly FishItem.Rarity rarity;

    public HookRarityEffect(FishItem.Rarity rarity)
    {
        this.rarity = rarity;
    }
    public void OnBeforeFishSelected(FishSelectionContext context)
    {
        context.forcedRarity = rarity;
    }
}
