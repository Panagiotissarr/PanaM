using UnityEngine;

namespace PanaM;

public class ModesTab : ITab
{
    public string name => "Modes";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.rgbMode = Widgets.Toggle(CheatToggles.rgbMode, "RGB Mode");

        CheatToggles.stealthMode = Widgets.Toggle(CheatToggles.stealthMode, "Stealth Mode");

        CheatToggles.panicMode = Widgets.Toggle(CheatToggles.panicMode, "Panic Mode");

        Widgets.EndSection();

        GUILayout.EndVertical();
    }
}
