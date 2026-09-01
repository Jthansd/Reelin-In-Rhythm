using UnityEngine;
using UnityEngine.InputSystem;

public class ShopTooltipUI : MonoBehaviour
{
    private RectTransform canvasTransform;
    [SerializeField] private RectTransform transformReference;
    [SerializeField] private Vector2 cursorOffset = new Vector2(20f, -20f);

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        UpdatePosition(Mouse.current.position.ReadValue());
    }

    public void Initialize(RectTransform canvas)
    {
        canvasTransform = canvas;
    }

    public void UpdatePosition(Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition + cursorOffset,
            null,
            out Vector2 targetLocalPoint
        );

        Vector2 referenceOffset = (Vector2)transformReference.localPosition;
        rect.anchoredPosition = targetLocalPoint - referenceOffset;
    }
}