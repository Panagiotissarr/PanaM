using UnityEngine;

namespace PanaM;

public static class UIHelpers
{
    // Resolves the active accent color (config value or RGB mode) into the theme
    public static void ApplyUIColor()
    {
        Theme.RefreshAccent();
    }
}
