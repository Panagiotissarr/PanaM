using UnityEngine;

namespace PanaM;

public class ConfigTab : ITab
{
    public string name => "Config";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.openConfig = Widgets.Toggle(CheatToggles.openConfig, "Open Config");

        CheatToggles.reloadConfig = Widgets.Toggle(CheatToggles.reloadConfig, "Reload Config");

        CheatToggles.saveProfile = Widgets.Toggle(CheatToggles.saveProfile, "Save to Profile");

        CheatToggles.loadProfile = Widgets.Toggle(CheatToggles.loadProfile, "Load from Profile");

        Widgets.EndSection();

        GUILayout.EndVertical();
    }
}
