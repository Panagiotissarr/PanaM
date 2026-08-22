using Il2CppSystem;
using UnityEngine;
using System.Collections.Generic;

namespace PanaM;

public class ConsoleUI : MonoBehaviour
{
    public static int windowHeight = 350;
    public static int windowWidth = 550;
    private Rect _windowRect;

    private GUIStyle _logStyle;
    private static Vector2 _scrollPosition = Vector2.zero;
    private static List<string> _logEntries = new();
    private const int MaxLogEntries = 300;

    private void Start()
    {
        // Instantiate 2D area of ConsoleUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showConsole || !(MenuUI.isGUIActive || PanaM.menuKeepSubwindowsOpen.Value) || PanaM.isPanicked) return;

        _logStyle ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 14
        };

        UIHelpers.ApplyUIColor();
        Theme.ApplySkinTheme();

        _windowRect = GUI.Window((int)WindowId.ConsoleUI, _windowRect, (GUI.WindowFunction)ConsoleWindow,
            GUIContent.none, Theme.InvisibleWindowStyle);
    }

    private void ConsoleWindow(int windowID)
    {
        var rect = new Rect(0, 0, _windowRect.width, _windowRect.height);

        Theme.DrawWindowChrome(rect);

        GUI.Label(new Rect(16, 10, rect.width - 32, 20), "CONSOLE", Theme.SectionStyle);

        GUILayout.Space(34);

        GUILayout.BeginHorizontal();
        GUILayout.Space(12);

        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);

        foreach (var log in _logEntries)
        {
            GUILayout.Label(log, _logStyle);
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();

        GUILayout.Space(12);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(12);

        if (Widgets.Button("Clear Log", GUILayout.Width(120), GUILayout.Height(26)))
        {
            _logEntries.Clear();
        }

        if (Widgets.Button("Copy Log to Clipboard", GUILayout.Width(170), GUILayout.Height(26)))
        {
            GUIUtility.systemCopyBuffer = String.Join("\n", _logEntries.ToArray());
        }

        GUILayout.Space(12);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.DragWindow(new Rect(0, 0, rect.width, 26));
    }

    public static void Log(string message)
    {
        if (_logEntries.Count >= MaxLogEntries) // Limit the number of logs to keep memory usage in check
        {
            _logEntries.RemoveAt(0); // Remove the oldest log entry
        }

        _logEntries.Add(message);

        // Scroll to the bottom
        _scrollPosition.y = float.MaxValue;
    }
}
