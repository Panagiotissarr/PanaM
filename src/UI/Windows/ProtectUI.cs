using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace PanaM;

public class ProtectUI : MonoBehaviour
{
    public static int windowHeight = 300;
    public static int windowWidth = 500;
    private Rect _windowRect;

    private Vector2 _scrollPosition = Vector2.zero;
    public static List<PlayerControl> playersToProtect = new();
    private bool _keepEveryoneProtected;

    private void Start()
    {
        // Instantiate 2D area of ProtectUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showProtectMenu || !(MenuUI.isGUIActive || PanaM.menuKeepSubwindowsOpen.Value) || PanaM.isPanicked) return;

        UIHelpers.ApplyUIColor();
        Theme.ApplySkinTheme();

        _windowRect = GUI.Window((int)WindowId.ProtectUI, _windowRect, (GUI.WindowFunction)ProtectWindow,
            GUIContent.none, Theme.InvisibleWindowStyle);
    }

    private void ProtectWindow(int windowID)
    {
        var rect = new Rect(0, 0, _windowRect.width, _windowRect.height);

        Theme.DrawWindowChrome(rect);

        GUI.Label(new Rect(16, 10, rect.width - 32, 20), "PROTECT PLAYERS", Theme.SectionStyle);

        GUILayout.Space(34);

        GUILayout.BeginHorizontal();
        GUILayout.Space(12);

        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || !player.Data.Role || string.IsNullOrEmpty(player.Data.PlayerName))
            {
                if (playersToProtect.Contains(player))  // Ensure to remove invalid players from the list
                {
                    playersToProtect.Remove(player);
                }

                continue;
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>", GUILayout.Width(140f));

            if (player.protectedByGuardianId == -1)
            {
                GUILayout.Label("<color=#F85149>Unprotected</color>", GUILayout.Width(135));
            }
            else
            {
                NetworkedPlayerInfo guardianInfo = GameData.Instance.GetPlayerById((byte)player.protectedByGuardianId);
                GUILayout.Label($"<color=#3FB950>Protected</color> by <color=#{ColorUtility.ToHtmlStringRGB(guardianInfo.Color)}>{guardianInfo._object.Data.PlayerName}</color>", GUILayout.Width(135));
            }

            if (Widgets.AccentButton("Protect", GUILayout.Width(80), GUILayout.Height(24)) && Utils.isHost && !Utils.isLobby)
            {
                PlayerControl.LocalPlayer.RpcProtectPlayer(player, player.cosmetics.ColorId);
            }

            var keepProtected = playersToProtect.Contains(player);
            keepProtected = Widgets.Toggle(keepProtected, "Keep");

            if (keepProtected && !playersToProtect.Contains(player))
            {
                playersToProtect.Add(player);
            }
            else if (!keepProtected && playersToProtect.Contains(player))
            {
                playersToProtect.Remove(player);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        if (Widgets.AccentButton("Protect Everyone", GUILayout.Height(26)) && Utils.isHost && !Utils.isLobby)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                PlayerControl.LocalPlayer.RpcProtectPlayer(player, player.cosmetics.ColorId);
            }
        }

        GUILayout.FlexibleSpace();

        _keepEveryoneProtected = Widgets.Toggle(_keepEveryoneProtected, "Keep Everyone Protected");

        if (_keepEveryoneProtected)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!playersToProtect.Contains(player))
                {
                    playersToProtect.Add(player);
                }
            }
        }
        else
        {
            if (PlayerControl.AllPlayerControls.Count == playersToProtect.Count)  // Only clear the list if all players were being kept protected
            {
                playersToProtect.Clear();
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(12);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.DragWindow(new Rect(0, 0, rect.width, 26));
    }
}
