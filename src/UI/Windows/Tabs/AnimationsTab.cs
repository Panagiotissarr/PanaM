using UnityEngine;

namespace PanaM;

public class AnimationsTab : ITab
{
    public string name => "Animations";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.animShields = Widgets.Toggle(CheatToggles.animShields, "Shields");

        CheatToggles.animAsteroids = Widgets.Toggle(CheatToggles.animAsteroids, "Asteroids");

        CheatToggles.animEmptyGarbage = Widgets.Toggle(CheatToggles.animEmptyGarbage, "Empty Garbage");

        CheatToggles.animMedScan = Widgets.Toggle(CheatToggles.animMedScan, "Medbay Scan");

        CheatToggles.animCamsInUse = Widgets.Toggle(CheatToggles.animCamsInUse, "Cams In Use");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Client-Sided");

        CheatToggles.moonWalk = Widgets.Toggle(CheatToggles.moonWalk, "Moonwalk");

        Widgets.EndSection();

        GUILayout.EndVertical();
    }
}
