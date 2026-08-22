using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace PanaM;

public class DoorsUI : MonoBehaviour
{
    public static int windowHeight = 270;
    public static int windowWidth = 480;
    private Rect _windowRect;

    private List<SystemTypes> _doorsToSpamOpen = new();
    private List<SystemTypes> _doorsToSpamClose = new();

    private void Start()
    {
        // Instantiate 2D area of DoorsUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showDoorsMenu || !(MenuUI.isGUIActive || PanaM.menuKeepSubwindowsOpen.Value) || PanaM.isPanicked) return;

        UIHelpers.ApplyUIColor();
        Theme.ApplySkinTheme();

        _windowRect = GUI.Window((int)WindowId.DoorsUI, _windowRect, (GUI.WindowFunction)DoorsWindow,
            GUIContent.none, Theme.InvisibleWindowStyle);
    }

    private void DoorsWindow(int windowID)
    {
        var rect = new Rect(0, 0, _windowRect.width, _windowRect.height);

        Theme.DrawWindowChrome(rect);

        GUI.Label(new Rect(16, 10, rect.width - 32, 20), "DOORS", Theme.SectionStyle);

        GUILayout.Space(34);

        if (!Utils.isShip)
        {
            GUI.DragWindow(new Rect(0, 0, rect.width, rect.height));
            return;
        }

        var map = (MapNames)Utils.GetCurrentMapID();

        if (map is MapNames.MiraHQ)
        {
            GUI.DragWindow(new Rect(0, 0, rect.width, rect.height));
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(12);

        GUILayout.BeginVertical();

        foreach (var doorRoom in DoorsHandler.GetRoomsWithDoors())
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{doorRoom.ToString()}", GUILayout.Width(110f));

            GUILayout.BeginHorizontal();

            GUILayout.Label($"{DoorsHandler.GetStatusOfDoorsInRoom(doorRoom, true)}");

            GUILayout.FlexibleSpace();

            if (Widgets.Button("Close", GUILayout.Width(56), GUILayout.Height(22)))
            {
                DoorsHandler.CloseDoorsInRoom(doorRoom);
            }

            if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
            {
                if (Widgets.Button("Open", GUILayout.Width(56), GUILayout.Height(22)))
                {
                    DoorsHandler.OpenDoorsInRoom(doorRoom);
                }
            }

            if (Utils.isHost)
            {
                var spamClose = _doorsToSpamClose.Contains(doorRoom);
                spamClose = Widgets.Toggle(spamClose, "S.Close");

                if (spamClose && !_doorsToSpamClose.Contains(doorRoom))
                {
                    _doorsToSpamClose.Add(doorRoom);
                }
                else if (!spamClose && _doorsToSpamClose.Contains(doorRoom))
                {
                    _doorsToSpamClose.Remove(doorRoom);
                }

                if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
                {
                    var spamOpen = _doorsToSpamOpen.Contains(doorRoom);
                    spamOpen = Widgets.Toggle(spamOpen, "S.Open");

                    if (spamOpen && !_doorsToSpamOpen.Contains(doorRoom))
                    {
                        _doorsToSpamOpen.Add(doorRoom);
                    }
                    else if (!spamOpen && _doorsToSpamOpen.Contains(doorRoom))
                    {
                        _doorsToSpamOpen.Remove(doorRoom);
                    }
                }
            }
            else
            {
                // Clear spam lists if not host
                if (_doorsToSpamClose.Count != 0 || _doorsToSpamOpen.Count != 0)
                {
                    _doorsToSpamClose.Clear();
                    _doorsToSpamOpen.Clear();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();

        Widgets.Divider();

        GUILayout.Space(8);

        GUILayout.BeginHorizontal();

        if (Widgets.Button("Close All", GUILayout.Height(26)))
        {
            CheatToggles.closeAllDoors = true;
        }

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            if (Widgets.Button("Open All", GUILayout.Height(26)))
            {
                CheatToggles.openAllDoors = true;
            }
        }

        GUILayout.FlexibleSpace();

        if (Utils.isHost)
        {
            CheatToggles.spamCloseAllDoors = Widgets.Toggle(CheatToggles.spamCloseAllDoors, "Spam Close All");

            if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
            {
                CheatToggles.spamOpenAllDoors = Widgets.Toggle(CheatToggles.spamOpenAllDoors, "Spam Open All");
            }
        }
        else
        {
            CheatToggles.spamCloseAllDoors = CheatToggles.spamOpenAllDoors = false;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(12);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.DragWindow(new Rect(0, 0, rect.width, 26));
    }

    public void Update()
    {
        if (!Utils.isShip) return;

        // Spam close selected doors
        foreach (var doorRoom in _doorsToSpamClose)
        {
            DoorsHandler.CloseDoorsInRoom(doorRoom);
        }

        // Spam open selected doors
        var map = (MapNames)Utils.GetCurrentMapID();

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            foreach (var doorRoom in _doorsToSpamOpen)
            {
                DoorsHandler.OpenDoorsInRoom(doorRoom);
            }
        }
    }
}
