using System;

public static class MenuStateEvents
{
    // true = a menu/UI is open (gameplay input should be restricted)
    // false = no menu open (normal gameplay input restored)
    public static event Action<bool> OnMenuStateChanged;

    public static void SetMenuOpen(bool isOpen)
    {
        OnMenuStateChanged?.Invoke(isOpen);
    }
}