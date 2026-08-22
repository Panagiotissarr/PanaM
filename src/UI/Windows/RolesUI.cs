using UnityEngine;

namespace PanaM;

public class RolesUI : MonoBehaviour
{
    public static int windowHeight = 130;
    public static int windowWidth = 450;
    private Rect _windowRect;

    private Vector2 _scrollPosition = Vector2.zero;

    private void Start()
    {
        // Instantiate 2D area of RolesUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showRolesMenu || !(MenuUI.isGUIActive || PanaM.menuKeepSubwindowsOpen.Value) || PanaM.isPanicked) return;

        UIHelpers.ApplyUIColor();
        Theme.ApplySkinTheme();

        _windowRect = GUI.Window((int)WindowId.RolesUI, _windowRect, (GUI.WindowFunction)RolesWindow,
            GUIContent.none, Theme.InvisibleWindowStyle);
    }

    private void RolesWindow(int windowID)
    {
        var rect = new Rect(0, 0, _windowRect.width, _windowRect.height);

        Theme.DrawWindowChrome(rect);

        GUI.Label(new Rect(16, 10, rect.width - 32, 20), "ASSIGN ROLES", Theme.SectionStyle);

        GUILayout.Space(34);

        GUILayout.BeginHorizontal();
        GUILayout.Space(12);

        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || !player.Data.Role || string.IsNullOrEmpty(player.Data.PlayerName) || player != PlayerControl.LocalPlayer) continue;

            GUILayout.BeginHorizontal();

            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>", GUILayout.Width(140f));
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{CheatToggles.forcedRole}");
            GUILayout.FlexibleSpace();

            if (Widgets.Button("Reset", GUILayout.Width(70f), GUILayout.Height(24f)))
            {
                CheatToggles.forcedRole = null;
            }
            if (Widgets.AccentButton("Assign", GUILayout.Width(70f), GUILayout.Height(24f)))
            {
                CheatToggles.forceRole = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        Widgets.MutedLabel("Roles will be assigned on next game start");

        GUILayout.EndVertical();

        GUILayout.Space(12);
        GUILayout.EndHorizontal();

        GUI.DragWindow(new Rect(0, 0, rect.width, 26));
    }
}
