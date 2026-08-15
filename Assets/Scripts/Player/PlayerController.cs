using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] MovementController movementController;
    [SerializeField] Fisherman fisherman;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] FishOPediaManager fishOPediaManager;


    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        fisherman = GetComponent<Fisherman>();
    }
    private void OnMove(InputValue movementValue)
    {
        Vector2 move = movementValue.Get<Vector2>();
        movementController.Move(move);
    }

    private void OnCast()
    {
        Debug.Log("Cast Triggered");
        fisherman.Cast();
    }

    private void OnToggleInventory()
    {
        Debug.Log("Inventory Toggle Triggered");
        inventoryManager.ToggleInventory();
    }

    private void OnToggleFishOPedia()
    {
        Debug.Log("FishOPedia Toggle Triggered");
        fishOPediaManager.ToggleFishOPedia();
    }

    private void OnTogglePause()
    {
        if (GamePauseManager.Instance.IsPaused)
        {
            GamePauseManager.Instance.ReleasePause();
        }
        else
        {
            GamePauseManager.Instance.RequestPause();
        }
    }

    private void OnToggleObjectives()
    {
        Debug.Log("Open the Objectives Menu");
        ObjectiveManager.Instance.ToggleObjectives();
    }

}
