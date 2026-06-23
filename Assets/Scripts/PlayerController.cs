using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] MovementController movementController;
    [SerializeField] Fisherman fisherman;


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

}
