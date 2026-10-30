using UnityEngine;

public class ControlToggler : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private Fisherman fisherman;
    [SerializeField] private PlayerController playerController;

    public void FreezeControls()
    {
        movementController.enabled = false;
        fisherman.enabled = false;
        playerController.enabled = false;
    }

    public void UnfreezeControls()
    {
        movementController.enabled = true;
        fisherman.enabled = true;
        playerController.enabled = true;
    }
}