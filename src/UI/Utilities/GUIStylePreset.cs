using UnityEngine;

namespace PanaM;

// Legacy style presets kept for compatibility with remaining call sites.
// All presets now resolve onto the modern Theme styles.
public static class GUIStylePreset
{
    private static GUIStyle _separator;
    private static GUIStyle _darkSeparator;
    private static GUIStyle _normalToggle;
    private static GUIStyle _tabButton;

    public static GUIStyle Separator => _separator ??= MakeDivider(Theme.DividerColor);

    public static GUIStyle DarkSeparator => _darkSeparator ??= MakeDivider(new Color(1f, 1f, 1f, 0.10f));

    public static GUIStyle NormalButton => Theme.ButtonStyle;

    public static GUIStyle NormalToggle
    {
        get
        {
            if (_normalToggle == null)
            {
                _normalToggle = new GUIStyle(GUI.skin.toggle)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Normal
                };
                _normalToggle.normal.textColor = Theme.TextPrimary;
                _normalToggle.hover.textColor = Color.white;
            }

            return _normalToggle;
        }
    }

    public static GUIStyle TabButton
    {
        get
        {
            if (_tabButton == null)
            {
                _tabButton = new GUIStyle(Theme.ButtonStyle)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold
                };
            }

            return _tabButton;
        }
    }

    public static GUIStyle TabTitle => Theme.TitleStyle;

    public static GUIStyle TabSubtitle
    {
        get
        {
            var s = Theme.SectionStyle;
            s.fontSize = 14;
            s.fontStyle = FontStyle.Bold;
            s.normal.textColor = Theme.TextSecondary;
            return s;
        }
    }

    private static GUIStyle MakeDivider(Color color)
    {
        var style = new GUIStyle(GUI.skin.box)
        {
            margin = new RectOffset { top = 4, bottom = 4 },
            padding = new RectOffset(),
            border = new RectOffset()
        };
        style.normal.background = Theme.SolidTinted(color);
        return style;
    }
}
