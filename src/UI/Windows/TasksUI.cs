using System.Linq;
using UnityEngine;

namespace PanaM;

public class TasksUI : MonoBehaviour
{
    public static int windowHeight = 300;
    public static int windowWidth = 500;
    private Rect _windowRect;

    private Vector2 _scrollPosition = Vector2.zero;
    private GUIStyle _playerHeaderStyle;
    private Il2CppSystem.Text.StringBuilder _tasksString = new();
    private readonly System.Collections.Generic.Dictionary<string, bool> _expandedPlayers = new();

    private void Start()
    {
        // Instantiate 2D area of TasksUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showTasksMenu || !(MenuUI.isGUIActive || PanaM.menuKeepSubwindowsOpen.Value) || PanaM.isPanicked) return;

        if (_playerHeaderStyle == null)
        {
            _playerHeaderStyle = new GUIStyle(Theme.ButtonStyle)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft
            };
        }

        UIHelpers.ApplyUIColor();
        Theme.ApplySkinTheme();

        _windowRect = GUI.Window((int)WindowId.TasksUI, _windowRect, (GUI.WindowFunction)TasksWindow,
            GUIContent.none, Theme.InvisibleWindowStyle);
    }

    private void TasksWindow(int windowID)
    {
        var rect = new Rect(0, 0, _windowRect.width, _windowRect.height);

        Theme.DrawWindowChrome(rect);

        GUI.Label(new Rect(16, 10, rect.width - 32, 20), "TASKS", Theme.SectionStyle);

        GUILayout.Space(34);

        GUILayout.BeginHorizontal();
        GUILayout.Space(12);

        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || !player.Data.Role || string.IsNullOrEmpty(player.Data.PlayerName)) continue;

            GUILayout.BeginVertical();

            var nameKey = player.Data.PlayerName;
            _expandedPlayers.TryGetValue(nameKey, out var expanded);
            var arrow = expanded ? "\u25BC" : "\u25B6"; // ▼ or ▶

            var taskCount = player.myTasks.Count;
            var completeCount = player.myTasks.ToArray().Count(t => t.IsComplete);

            if (player == PlayerControl.LocalPlayer && player.Data.IsDead)
            {
                taskCount -= 1;
            }
            if (player == PlayerControl.LocalPlayer && Utils.isAnySabotageActive)
            {
                taskCount -= 1;
            }
            if (player == PlayerControl.LocalPlayer && player.Data.Role.IsImpostor)
            {
                taskCount -= 1;
            }

            if (GUILayout.Button($"{arrow} [{completeCount}/{taskCount}] <color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{nameKey}</color>", _playerHeaderStyle))
            {
                _expandedPlayers[nameKey] = !expanded;
                expanded = !expanded;
            }

            if (expanded)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();

                foreach (var task in player.myTasks)
                {
                    // Do some checks to not show texts: sabotage active, dead hint, impostor hint
                    if (task.TaskType is TaskTypes.ResetReactor or TaskTypes.RestoreOxy or TaskTypes.FixLights or TaskTypes.FixComms or TaskTypes.ResetSeismic or TaskTypes.StopCharles or TaskTypes.MushroomMixupSabotage) continue;

                    _tasksString.Clear();
                    task.AppendTaskText(_tasksString);
                    var taskText = _tasksString.ToString();

                    if (taskText.Contains("You're dead") || taskText.Contains("Sabotage and kill")) continue;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(taskText.Replace("\n", "").Replace("</color>", "").Replace("<color=#00DD00FF>", "").Replace("<color=#FFFF00FF>", ""));
                    GUILayout.FlexibleSpace();

                    if (task.IsComplete)
                    {
                        var doneStyle = Theme.MutedStyle;
                        doneStyle.normal.textColor = Theme.SuccessColor;
                        GUI.Label(GUILayoutUtility.GetRect(80, 18), "<color=#3FB950>✔ Complete</color>", doneStyle);
                    }
                    else
                    {
                        if (player == PlayerControl.LocalPlayer)
                        {
                            if (Widgets.Button("Complete", GUILayout.Width(90), GUILayout.Height(22)))
                            {
                                Utils.CompleteTask(task);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();

        GUILayout.Space(6);

        if (Widgets.AccentButton("Complete My Tasks", GUILayout.Height(28)))
        {
            CheatToggles.completeMyTasks = true;
        }

        GUILayout.EndVertical();

        GUILayout.Space(12);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.DragWindow(new Rect(0, 0, rect.width, 26));
    }
}
