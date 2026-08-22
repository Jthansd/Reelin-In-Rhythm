using System;
using UnityEngine;

public static class MenuStateEvents
{
    private static int openRequests = 0;

    public static bool IsOpen => openRequests > 0;

    public static event Action<bool> OnMenuStateChanged;

    public static void SetMenuOpen(bool isOpen)
    {
        if (isOpen)
        {
            if (openRequests == 0)
            {
                OnMenuStateChanged?.Invoke(true);
            }
            openRequests++;
        }
        else
        {
            if (openRequests <= 0) return;
            openRequests--;
            if (openRequests == 0)
            {
                OnMenuStateChanged?.Invoke(false);
            }
        }
        Debug.Log($"MenuStateEvents: openRequests = {openRequests}");
    }
}