using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour, IPopup
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button dismissButton;
    private Action onDismissed;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Dismiss();
        }
    }

    public void ForceDismiss()
    {
        Dismiss(); // reuses existing private method - just needs to become accessible, or ForceDismiss() calls the same body
    }

    public void Show(string message, Action onDismissedCallback)
    {
        messageText.text = message;
        onDismissed = onDismissedCallback;
        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(Dismiss);
        }
        MenuStateEvents.SetMenuOpen(true);
    }

    private void Dismiss()
    {
        MenuStateEvents.SetMenuOpen(false);
        onDismissed?.Invoke();
        Destroy(gameObject);
    }
}