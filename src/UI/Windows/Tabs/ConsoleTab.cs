using UnityEngine;

namespace PanaM;

public class ConsoleTab : ITab
{
    public string name => "Console";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.showConsole = Widgets.Toggle(CheatToggles.showConsole, "Show Console");

        CheatToggles.logDeaths = Widgets.Toggle(CheatToggles.logDeaths, "Log Deaths");

        CheatToggles.logShapeshifts = Widgets.Toggle(CheatToggles.logShapeshifts, "Log Shapeshifts");

        CheatToggles.logVents = Widgets.Toggle(CheatToggles.logVents, "Log Vents");

        Widgets.EndSection();

        GUILayout.EndVertical();
    }
}
