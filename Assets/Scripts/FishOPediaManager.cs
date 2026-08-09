using UnityEngine;

public class FishOPediaManager : MonoBehaviour
{
    [SerializeField] GameObject fishOPedia;
    [SerializeField] FishOPediaInventoryUI leftPage;
    [SerializeField] FishOPediaInventoryUI rightPage;

    public void ToggleFishOPedia()
    {
        bool enabled = !fishOPedia.activeSelf;
        fishOPedia.SetActive(enabled);
        CameraOrbit.Instance.SetLookEnabled(!enabled);
    }
}