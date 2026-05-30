using UnityEngine;

public static class CursorManager
{
    public static void SetGameplayMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void SetMenuMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}