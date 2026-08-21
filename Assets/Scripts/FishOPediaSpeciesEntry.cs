using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishOPediaSpeciesEntry : MonoBehaviour
{

    [SerializeField] private Image entryIcon;
    [SerializeField] private TextMeshProUGUI entryNameText;
    [SerializeField] private TextMeshProUGUI entryDescriptionText;
    [SerializeField] private TextMeshProUGUI biggestSizeText;
    [SerializeField] private TextMeshProUGUI smallestSizeText;
    [SerializeField] private List<Image>  rarityContainers;


    private List<FishItem.Rarity> possibileRarities;
    private List<FishItem.Rarity> discoveredRarities;
    private FishItem.FishSize smallestSize;
    private FishItem.FishSize biggestSize;
    private float biggestSizeValue;
    private float smallestSizeValue;
    private string speciesName;
    private string speciesDescription;
    private Sprite speciesIcon;
    private string undiscoveredText;

    public void SetEntryComponents(FishItem species, float bigValue, float smallValue, FishItem.FishSize bigFishSize, FishItem.FishSize smallFishSize, List<FishItem.Rarity> discoveredRarityList, bool discovered)
    {
        biggestSizeText.text = bigValue.ToString();
        smallestSizeText.text = smallValue.ToString();
        entryIcon.sprite = species.Icon;
        SetRarities(rarityContainers, species.PossibleRarities, discoveredRarityList);


        if (discovered)
        {
            entryNameText.text = species.name;
            entryDescriptionText.text = species.ItemDescription;


            smallestSize = smallFishSize;
            biggestSize = bigFishSize;

        }
        else
        {
            speciesName = undiscoveredText;
            speciesDescription = undiscoveredText;

            //hide the icon
            entryIcon.color = new Color(0f, 0f, 0f, 1f); 

        }
        
        Debug.Log($"Built a fishopedia species entry with components:\nSpecies Name: {speciesName}\nSpecies Description: {speciesDescription}\nBiggest Size: {biggestSizeValue}\nSmallest Size: {smallestSizeValue}\nPossible Rarities: {possibileRarities}\nDiscovered Rarities: {discoveredRarities}");
    }

    private void SetRarities(List<Image> rarityContainers, List<FishItem.Rarity> possibleRarities, List<FishItem.Rarity> discoveredRarities)
    {
        int index = 0;
        foreach (Image rarityContainer in rarityContainers)
        {
            FishItem.Rarity currentRarity = (FishItem.Rarity)Enum.GetValues(typeof(FishItem.Rarity)).GetValue(index);
            Color baseColor = FishItem.RarityColor(currentRarity);

            if (possibleRarities.Contains(currentRarity))
            {
                rarityContainer.color = baseColor * 0.5f;
            }
            if (discoveredRarities.Contains(currentRarity))
            {
                rarityContainer.color = baseColor;
            }

            index++;
        }
    }

}
