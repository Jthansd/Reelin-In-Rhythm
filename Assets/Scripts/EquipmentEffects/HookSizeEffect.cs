using UnityEngine;

public class HookSizeEffect : IEquipmentEffect
{
    public readonly FishItem.FishSize size;
    public HookSizeEffect(FishItem.FishSize effectSizeParam)
    {
        size = effectSizeParam;
    }

    public void OnBeforeFishSelected(FishSelectionContext context)
    {
        context.forcedSize = size;
    }

}