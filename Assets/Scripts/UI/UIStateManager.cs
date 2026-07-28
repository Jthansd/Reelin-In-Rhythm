using TMPro;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hookedText;
    [SerializeField] FishingController fishingController;

    private void Start()
    {
        hookedText.gameObject.SetActive(false);
    }

    private void Update()
    {
        hookedText.gameObject.SetActive(fishingController.hooked);
    }
}
