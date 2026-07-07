using UnityEngine;

public class MovementController : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedStickForce = -2f;

    private Vector2 moveInput;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        ApplyGravity();
        ApplyMovement();
        ApplyRotation();
    }

    public void Move(Vector2 move)
    {
        moveInput = move;
    }

    private void ApplyMovement()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        moveDir = Vector3.ClampMagnitude(moveDir, 1f); // prevent diagonal speed boost

        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            // small constant downward force keeps the controller "stuck" to slopes
            verticalVelocity = groundedStickForce;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void ApplyRotation()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public float GetHorizontalSpeedPercent()
    {
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        return Mathf.Clamp01(horizontalVelocity.magnitude / moveSpeed);
    }
}