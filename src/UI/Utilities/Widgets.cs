using UnityEngine;

namespace PanaM;

public static class Widgets
{
    public static bool Toggle(bool value, string label, float indent = 0f)
    {
        var rect = GUILayoutUtility.GetRect(0, Theme.RowHeight, GUILayout.ExpandWidth(true));
        var row = new Rect(rect.x + indent, rect.y, rect.width - indent, rect.height);

        var e = Event.current;
        bool clicked = e.type == EventType.MouseDown && e.button == 0 && row.Contains(e.mousePosition);
        if (clicked) e.Use();

        bool hover = row.Contains(e.mousePosition);
        if (hover && !clicked)
        {
            Theme.DrawRounded(row, 6f, Theme.SurfaceIdle);
        }

        var textRect = new Rect(row.x, row.y, row.width - Theme.SwitchWidth - 10f, row.height);
        var style = Theme.BodyStyle;
        var oldColor = style.normal.textColor;
        style.normal.textColor = hover ? Color.white : Theme.TextPrimary;
        GUI.Label(textRect, label, style);
        style.normal.textColor = oldColor;

        DrawSwitch(new Rect(row.xMax - Theme.SwitchWidth, row.y + (row.height - Theme.SwitchHeight) / 2f,
            Theme.SwitchWidth, Theme.SwitchHeight), value);

        return clicked ? !value : value;
    }

    public static void DrawSwitch(Rect trackRect, bool on)
    {
        Theme.DrawRounded(trackRect, Theme.SwitchHeight / 2f, on ? Theme.Accent : new Color(1f, 1f, 1f, 0.14f));

        float knob = Theme.SwitchHeight - 4f;
        float x = on ? trackRect.xMax - knob - 2f : trackRect.x + 2f;
        var knobRect = new Rect(x, trackRect.y + 2f, knob, knob);
        Theme.DrawCircle(knobRect, Color.white);
    }

    public static void BeginSection(string title = null)
    {
        GUILayout.BeginVertical(Theme.CardStyle, GUILayout.ExpandWidth(true));

        if (!string.IsNullOrEmpty(title))
        {
            GUILayout.Space(2);
            var titleRect = GUILayoutUtility.GetRect(0, 18, GUILayout.ExpandWidth(true));
            GUI.Label(new Rect(titleRect.x + 2, titleRect.y, titleRect.width, titleRect.height),
                title.ToUpperInvariant(), Theme.SectionStyle);
            GUILayout.Space(4);
        }
    }

    public static void EndSection()
    {
        GUILayout.Space(2);
        GUILayout.EndVertical();
    }

    public static bool Button(string label, params GUILayoutOption[] options)
    {
        return GUILayout.Button(label, Theme.ButtonStyle, options);
    }

    public static bool AccentButton(string label, params GUILayoutOption[] options)
    {
        var old = GUI.backgroundColor;
        GUI.backgroundColor = Theme.Accent;
        bool pressed = GUILayout.Button(label, Theme.ButtonStyle, options);
        GUI.backgroundColor = old;
        return pressed;
    }

    public static bool DangerButton(string label, params GUILayoutOption[] options)
    {
        var old = GUI.backgroundColor;
        GUI.backgroundColor = Theme.DangerColor;
        bool pressed = GUILayout.Button(label, Theme.ButtonStyle, options);
        GUI.backgroundColor = old;
        return pressed;
    }

    public static void Label(string text, GUIStyle style = null)
    {
        GUILayout.Label(text, style ?? Theme.BodyStyle);
    }

    public static void MutedLabel(string text)
    {
        GUILayout.Label(text, Theme.MutedStyle);
    }

    public static void Divider()
    {
        var rect = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
        Theme.DrawRect(rect, Theme.DividerColor);
    }

    public static Vector2 BeginScrollView(Vector2 scroll, params GUILayoutOption[] options)
    {
        return GUILayout.BeginScrollView(scroll, false, true, options);
    }

    public static void EndScrollView()
    {
        GUILayout.EndScrollView();
    }
}
