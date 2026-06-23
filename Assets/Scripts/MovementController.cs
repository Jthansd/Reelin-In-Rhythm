using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] public float acceleration = 10f;
    Vector2 moveInput;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxVelocity = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ClampVelocity();
        ApplyRotation();

        //Vector3 raw = new Vector3(moveInput.x, 0f, moveInput.y);
        //Vector3 projected = ProjectOnSlope(raw);

        //Debug.DrawRay(transform.position, raw * 100f, Color.red);
        //Debug.DrawRay(transform.position, projected * 100f, Color.green);


        // Grounding force
        //rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
    }

    public void Move(Vector2 move)
    {
        moveInput = move;
    }

    private Vector3 ProjectOnSlope(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f))
        {
            Debug.Log("Slope normal: " + hit.normal);
            return Vector3.ProjectOnPlane(direction, hit.normal).normalized;
        }



        return direction;
    }

    private void ApplyMovement()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        if (movement.sqrMagnitude > 0.01f)
        {
            movement = ProjectOnSlope(movement);
            rb.AddForce(movement * acceleration, ForceMode.Acceleration);
        }
    }

    void ApplyRotation()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.magnitude > 0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    public float GetHorizontalSpeedPercent()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        return Mathf.Clamp01(horizontalVelocity.magnitude / maxVelocity);
    }

    void ClampVelocity()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > maxVelocity)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }
    }
}
