using UnityEngine;
using System;

namespace PanaM;

public class MovementTab : ITab
{
    public string name => "Movement";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(4);

        DrawTeleport();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        Widgets.BeginSection("General");

        CheatToggles.noClip = Widgets.Toggle(CheatToggles.noClip, "NoClip");

        CheatToggles.invertControls = Widgets.Toggle(CheatToggles.invertControls, "Invert Controls");

        try
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed, 0f, 20f, GUILayout.Width(250f));
                Utils.SnapSpeedToDefault(0.05f, true);
                GUILayout.Label($"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.GhostSpeed} {(Utils.IsSpeedDefault(true) ? "(Default)" : "")}");
            }
            else
            {
                PlayerControl.LocalPlayer.MyPhysics.Speed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.Speed, 0f, 20f, GUILayout.Width(250f));
                Utils.SnapSpeedToDefault(0.05f);
                GUILayout.Label($"Current Speed: {PlayerControl.LocalPlayer?.MyPhysics.Speed} {(Utils.IsSpeedDefault() ? "(Default)" : "")}");
            }
        } catch (NullReferenceException) {}

        Widgets.EndSection();
    }

    private void DrawTeleport()
    {
        Widgets.BeginSection("Teleport");

        CheatToggles.teleportCursor = Widgets.Toggle(CheatToggles.teleportCursor, "to Cursor");

        CheatToggles.teleportPlayer = Widgets.Toggle(CheatToggles.teleportPlayer, "to Player");

        Widgets.EndSection();
    }
}
