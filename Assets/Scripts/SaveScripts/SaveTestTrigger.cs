using UnityEngine;
using UnityEngine.InputSystem;

public class SaveTestTrigger : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            SaveManager.Instance.Save();
        }

        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            SaveManager.Instance.Load();
        }
    }
}