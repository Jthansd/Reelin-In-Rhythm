using UnityEngine;

public static class CaughtFishBuilder
{

    public static CaughtFish BuildFish(FishItem species, FishItem.Rarity rarity)
    {
        CaughtFish hookedFish = new CaughtFish();
        //determine FishSize and size, then calculate difficulty
        FishItem.FishSize fishSize;
        float fishSizeValue;
        float minSize = species.MinSize;
        float maxSize = species.MaxSize;

        //calculate size

        fishSizeValue = FishingMath.CalculateFishSize(minSize, maxSize);
        fishSize = FishingMath.GetFishSizeCategory(fishSizeValue, maxSize);

        //calculate the sell price
        int sellPrice = FishingMath.CalculateSellValue(species.BaseValue, rarity, fishSize);

        return Build(species, fishSize, fishSizeValue, rarity, sellPrice);
    }
    public static CaughtFish Build(FishItem species, FishItem.FishSize size, float fishSizeValue, FishItem.Rarity rarity, int sellPrice)
    {
        CaughtFish result = new CaughtFish();
        result.species = species;
        result.fishSize = size;
        result.size = fishSizeValue;
        result.rarity = rarity;
        result.sellPrice = sellPrice;
        return result;
    }

}
