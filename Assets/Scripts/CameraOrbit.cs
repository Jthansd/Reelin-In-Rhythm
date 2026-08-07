using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    public static CameraOrbit Instance { get; private set; }

    [SerializeField] private Transform player;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private Vector3 lookTargetOffset = new(0f, 1.5f, 0f); // aim roughly at chest/head height

    private float yaw;
    private float pitch = 20f;
    private bool lookEnabled = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        MenuStateEvents.OnMenuStateChanged += HandleMenuStateChanged;
    }

    private void OnDisable()
    {
        MenuStateEvents.OnMenuStateChanged -= HandleMenuStateChanged;
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        SetLookEnabled(!menuOpen);
    }

    void Start()
    {
        Debug.Assert(player, "CameraOrbit requires a player Transform.");
        SetLookEnabled(true);
    }

    void LateUpdate()
    {
        if (lookEnabled)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * mouseSensitivity * 0.1f;
            pitch -= mouseDelta.y * mouseSensitivity * 0.1f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = player.position + lookTargetOffset;
        Vector3 desiredPosition = focusPoint - (rotation * Vector3.forward * distance);

        transform.position = desiredPosition;
        transform.rotation = rotation;
    }

    public void SetLookEnabled(bool enabled)
    {
        lookEnabled = enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }

    // Handy for movement script: horizontal-only facing direction of the camera
    public Vector3 GetFlatForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Vector3 GetFlatRight()
    {
        Vector3 right = transform.right;
        right.y = 0f;
        return right.normalized;
    }
}