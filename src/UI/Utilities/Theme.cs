using System;
using System.Collections.Generic;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace PanaM;

public static class Theme
{
    public const string DefaultAccentHex = "#4F8CFF";

    public const int RadiusPanel = 12;
    public const int RadiusCard = 10;
    public const int RadiusButton = 8;
    public const int TitleBarHeight = 38;
    public const int SearchFieldHeight = 30;
    public const float RowHeight = 26f;
    public const float SwitchWidth = 36f;
    public const float SwitchHeight = 18f;

    public static readonly Color GlassColor = FromHex("#0D1117");
    public static readonly Color TextPrimary = FromHex("#E8EAF0");
    public static readonly Color TextSecondary = FromHex("#9AA3B5");
    public static readonly Color TextMuted = FromHex("#6B7280");
    public static readonly Color HairlineColor = new(1f, 1f, 1f, 0.09f);
    public static readonly Color DividerColor = new(1f, 1f, 1f, 0.06f);
    public static readonly Color SurfaceIdle = new(1f, 1f, 1f, 0.055f);
    public static readonly Color SurfaceHover = new(1f, 1f, 1f, 0.095f);
    public static readonly Color SurfacePressed = new(1f, 1f, 1f, 0.14f);
    public static readonly Color SuccessColor = FromHex("#3FB950");
    public static readonly Color DangerColor = FromHex("#F85149");
    public static readonly Color WarningColor = FromHex("#D29922");

    private static Color _accent = FromHex(DefaultAccentHex);

    public static Color Accent => _accent;
    public static Color AccentSoft => new(_accent.r, _accent.g, _accent.b, 0.18f);

    public static float GlassOpacity => PanaM.menuGlassOpacity != null ? PanaM.menuGlassOpacity.Value : 0.86f;
    public static bool BlurEnabled => PanaM.menuBackdropBlur == null || PanaM.menuBackdropBlur.Value;

    public static Font UIFont { get; private set; }

    private static bool _fontTried;
    private static Texture2D _white;
    private static Texture2D _frostTex;
    private static Texture2D _shadowTex;
    private static Texture2D _scrollThumbTex;
    private static Texture2D _sliderThumbTex;
    private static Texture2D _sliderTrackTex;
    private static readonly Dictionary<int, Texture2D> RoundedTextures = new();
    private static readonly Dictionary<(int, int), GUIStyle> SliceStyles = new();

    private static GUIStyle _titleStyle;
    private static GUIStyle _sectionStyle;
    private static GUIStyle _bodyStyle;
    private static GUIStyle _mutedStyle;
    private static GUIStyle _buttonStyle;
    private static GUIStyle _cardStyle;
    private static GUIStyle _invisibleWindow;

    public static void RefreshAccent()
    {
        if (CheatToggles.rgbMode)
        {
            _accent = Color.HSVToRGB(MenuUI.hue % 1f, 0.62f, 1f);
            return;
        }

        var html = PanaM.menuHtmlColor != null ? PanaM.menuHtmlColor.Value : null;
        if (!string.IsNullOrWhiteSpace(html))
        {
            if (!html.StartsWith("#")) html = "#" + html;
            if (ColorUtility.TryParseHtmlString(html, out var parsed))
            {
                _accent = parsed;
                return;
            }
        }

        _accent = FromHex(DefaultAccentHex);
    }

    public static Color FromHex(string hex)
    {
        return ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.white;
    }

    private static void EnsureFont()
    {
        if (_fontTried) return;
        _fontTried = true;
        try
        {
            UIFont = Font.CreateDynamicFontFromOSFont("Segoe UI", 14);
        }
        catch
        {
            UIFont = null;
        }
    }

    private static float SdRoundRect(float px, float py, float halfW, float halfH, float r)
    {
        float qx = Math.Abs(px) - (halfW - r);
        float qy = Math.Abs(py) - (halfH - r);
        float ax = Math.Max(qx, 0f);
        float ay = Math.Max(qy, 0f);
        return (float)Math.Sqrt(ax * ax + ay * ay) + Math.Min(Math.Max(qx, qy), 0f) - r;
    }

    private static Il2CppStructArray<Color32> AllocPixels(int w, int h)
    {
        return new Il2CppStructArray<Color32>((long)w * h);
    }

    private static Texture2D Finish(Texture2D tex, Il2CppStructArray<Color32> pixels, bool repeat = false)
    {
        tex.wrapMode = repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(pixels);
        tex.Apply(false, true);
        return tex;
    }

    private static Texture2D SolidTexture()
    {
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var pixels = AllocPixels(4, 4);
        for (int i = 0; i < 16; i++) pixels[i] = new Color32(255, 255, 255, 255);
        return Finish(tex, pixels);
    }

    public static Texture2D SolidTinted(Color color)
    {
        EnsureCore();
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var pixels = AllocPixels(4, 4);
        for (int i = 0; i < 16; i++)
        {
            pixels[i] = new Color32((byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), (byte)(color.a * 255f));
        }
        return Finish(tex, pixels);
    }

    private static Texture2D MakeRounded(int w, int h, float radius, Color fill, Color stroke, int strokeW)
    {
        const int ss = 3;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = AllocPixels(w, h);
        float halfW = w / 2f;
        float halfH = h / 2f;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float fillCov = 0f;
                float strokeCov = 0f;

                for (int sy = 0; sy < ss; sy++)
                {
                    for (int sx = 0; sx < ss; sx++)
                    {
                        float fx = x + (sx + 0.5f) / ss - halfW;
                        float fy = y + (sy + 0.5f) / ss - halfH;
                        float sd = SdRoundRect(fx, fy, halfW - 0.75f, halfH - 0.75f, radius);

                        if (sd <= 0f)
                        {
                            fillCov += 1f / (ss * ss);
                            if (strokeW > 0 && sd >= -(strokeW + 0.5f)) strokeCov += 1f / (ss * ss);
                        }
                    }
                }

                if (fillCov <= 0.001f)
                {
                    pixels[y * w + x] = new Color32(0, 0, 0, 0);
                    continue;
                }

                float mix = Mathf.Clamp01(strokeCov / fillCov);
                byte r = (byte)((fill.r + (stroke.r - fill.r) * mix) * 255f);
                byte g = (byte)((fill.g + (stroke.g - fill.g) * mix) * 255f);
                byte b = (byte)((fill.b + (stroke.b - fill.b) * mix) * 255f);
                byte a = (byte)(fillCov * (fill.a + (stroke.a - fill.a) * mix) * 255f);
                pixels[y * w + x] = new Color32(r, g, b, a);
            }
        }

        return Finish(tex, pixels);
    }

    private static Texture2D MakeShadow()
    {
        int size = 96;
        float margin = 14f;
        float innerHalf = size / 2f - margin;
        const float strength = 0.42f;

        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = AllocPixels(size, size);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float fx = x + 0.5f - size / 2f;
                float fy = y + 0.5f - size / 2f;
                float sd = SdRoundRect(fx, fy, innerHalf, innerHalf, 6f);
                float t = Mathf.Clamp01(1f - sd / margin);
                pixels[y * size + x] = new Color(0f, 0f, 0f, t * t * strength);
            }
        }

        return Finish(tex, pixels);
    }

    private static Texture2D MakeFrost(int size, float radius)
    {
        const int ss = 3;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = AllocPixels(size, size);
        var rng = new System.Random(20260822);
        float halfW = size / 2f;
        float halfH = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float cov = 0f;
                for (int sy = 0; sy < ss; sy++)
                {
                    for (int sx = 0; sx < ss; sx++)
                    {
                        float fx = x + (sx + 0.5f) / ss - halfW;
                        float fy = y + (sy + 0.5f) / ss - halfH;
                        if (SdRoundRect(fx, fy, halfW - 0.75f, halfH - 0.75f, radius) <= 0f) cov += 1f / (ss * ss);
                    }
                }

                double grain = (rng.NextDouble() - 0.5) * 16.0;
                byte lum = (byte)Math.Clamp(248 + grain, 0, 255);
                pixels[y * size + x] = new Color32(lum, lum, lum, (byte)(cov * 255f));
            }
        }

        return Finish(tex, pixels);
    }

    private static Texture2D MakeCircle(int size, byte alpha)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = AllocPixels(size, size);
        float center = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - center;
                float dy = y + 0.5f - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(center - dist + 0.5f);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * alpha));
            }
        }

        return Finish(tex, pixels);
    }

    public static Texture2D WhiteTexture => EnsureCore();

    private static Texture2D EnsureCore()
    {
        EnsureFont();
        if (_white != null) return _white;

        _white = SolidTexture();
        _frostTex = MakeFrost(128, RadiusPanel);
        _shadowTex = MakeShadow();
        _scrollThumbTex = MakeRounded(24, 24, 3f, new Color(1f, 1f, 1f, 0.30f), default, 0);
        _sliderThumbTex = MakeCircle(20, 240);
        _sliderTrackTex = MakeRounded(24, 8, 4f, new Color(1f, 1f, 1f, 0.12f), default, 0);
        return _white;
    }

    private static Texture2D GetRounded(int radius)
    {
        EnsureCore();
        if (RoundedTextures.TryGetValue(radius, out var cached)) return cached;

        int size = radius * 2 + 12;
        var tex = MakeRounded(size, size, radius, Color.white, default, 0);
        RoundedTextures[radius] = tex;
        return tex;
    }

    private static GUIStyle Slice(Texture2D tex, int border)
    {
        var key = (tex.GetInstanceID(), border);
        if (SliceStyles.TryGetValue(key, out var cached)) return cached;

        var s = new GUIStyle
        {
            name = "PanaMSlice",
            border = new RectOffset { left = border, right = border, top = border, bottom = border },
            padding = new RectOffset(),
            margin = new RectOffset()
        };
        s.normal.background = tex;
        s.hover.background = tex;
        s.active.background = tex;
        s.focused.background = tex;
        SliceStyles[key] = s;
        return s;
    }

    private static GUIStyle MakeLabel(int size, FontStyle style, Color color, TextAnchor anchor)
    {
        EnsureFont();
        var s = new GUIStyle
        {
            font = UIFont,
            fontSize = size,
            fontStyle = style,
            alignment = anchor
        };
        s.normal.textColor = color;
        s.hover.textColor = color;
        s.active.textColor = color;
        return s;
    }

    public static GUIStyle TitleStyle => _titleStyle ??= MakeLabel(19, FontStyle.Bold, TextPrimary, TextAnchor.MiddleLeft);
    public static GUIStyle SectionStyle => _sectionStyle ??= MakeLabel(12, FontStyle.Bold, TextSecondary, TextAnchor.MiddleLeft);
    public static GUIStyle BodyStyle => _bodyStyle ??= MakeLabel(14, FontStyle.Normal, TextPrimary, TextAnchor.MiddleLeft);
    public static GUIStyle MutedStyle => _mutedStyle ??= MakeLabel(12, FontStyle.Normal, TextMuted, TextAnchor.MiddleLeft);

    public static GUIStyle ButtonStyle
    {
        get
        {
            if (_buttonStyle != null) return _buttonStyle;
            EnsureCore();

            var idle = MakeRounded(40, 40, RadiusButton, SurfaceIdle, default, 0);
            var hover = MakeRounded(40, 40, RadiusButton, SurfaceHover, default, 0);
            var pressed = MakeRounded(40, 40, RadiusButton, SurfacePressed, default, 0);

            _buttonStyle = new GUIStyle
            {
                font = UIFont,
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset { left = RadiusButton + 2, right = RadiusButton + 2, top = RadiusButton + 2, bottom = RadiusButton + 2 },
                padding = new RectOffset { left = 10, right = 10, top = 6, bottom = 6 }
            };
            _buttonStyle.normal.background = idle;
            _buttonStyle.normal.textColor = TextPrimary;
            _buttonStyle.hover.background = hover;
            _buttonStyle.hover.textColor = TextPrimary;
            _buttonStyle.active.background = pressed;
            _buttonStyle.active.textColor = TextPrimary;
            _buttonStyle.focused.background = idle;
            _buttonStyle.focused.textColor = TextPrimary;
            return _buttonStyle;
        }
    }

    public static GUIStyle InvisibleWindowStyle => _invisibleWindow ??= new GUIStyle();

    public static GUIStyle CardStyle
    {
        get
        {
            if (_cardStyle != null) return _cardStyle;
            EnsureCore();

            var tex = MakeRounded(64, 64, RadiusCard, new Color(1f, 1f, 1f, 0.045f), HairlineColor, 1);

            _cardStyle = new GUIStyle
            {
                border = new RectOffset { left = RadiusCard + 2, right = RadiusCard + 2, top = RadiusCard + 2, bottom = RadiusCard + 2 },
                padding = new RectOffset { left = 12, right = 12, top = 10, bottom = 10 },
                margin = new RectOffset { left = 0, right = 0, top = 0, bottom = 10 }
            };
            _cardStyle.normal.background = tex;
            return _cardStyle;
        }
    }

    public static void DrawRect(Rect rect, Color color)
    {
        EnsureCore();
        var old = GUI.color;
        GUI.color = color;
        GUI.Box(rect, GUIContent.none, Slice(_white, 0));
        GUI.color = old;
    }

    private static Texture2D _circleTex;

    public static void DrawCircle(Rect rect, Color color)
    {
        EnsureCore();
        _circleTex ??= MakeCircle(48, 255);

        var old = GUI.color;
        GUI.color = color;
        GUI.Box(rect, GUIContent.none, Slice(_circleTex, 0));
        GUI.color = old;
    }

    public static void DrawRounded(Rect rect, float radius, Color color)
    {
        int r = Mathf.Max(2, Mathf.CeilToInt(radius));
        var old = GUI.color;
        GUI.color = color;
        GUI.Box(rect, GUIContent.none, Slice(GetRounded(r), r + 1));
        GUI.color = old;
    }

    public static void DrawShadow(Rect rect, float expansion = 10f, float alpha = 0.55f)
    {
        var old = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, alpha);
        GUI.Box(new Rect(rect.x - expansion, rect.y - expansion, rect.width + expansion * 2, rect.height + expansion * 2),
            GUIContent.none, Slice(_shadowTex, 15));
        GUI.color = old;
    }

    public static void DrawWindowChrome(Rect rect)
    {
        EnsureCore();
        DrawShadow(rect);

        if (BlurEnabled && BackdropBlur.CanDisplay)
        {
            BackdropBlur.DrawRegion(rect);
        }
        else
        {
            DrawRounded(rect, RadiusPanel, new Color(0.02f, 0.03f, 0.05f, 0.55f));
        }

        DrawRounded(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), RadiusPanel - 1,
            new Color(GlassColor.r, GlassColor.g, GlassColor.b, GlassOpacity));

        DrawRounded(rect, RadiusPanel, HairlineColor);

        var old = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.035f);
        GUI.Box(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4), GUIContent.none,
            Slice(_frostTex, RadiusPanel + 2));
        GUI.color = old;
    }

    public static void ApplySkinTheme()
    {
        EnsureCore();

        if (UIFont != null) GUI.skin.font = UIFont;

        GUI.skin.label.fontSize = 14;
        GUI.skin.label.normal.textColor = TextPrimary;
        GUI.skin.box.fontSize = 13;
        GUI.skin.box.normal.textColor = TextPrimary;
        GUI.skin.button.fontSize = 13;
        GUI.skin.button.normal.textColor = TextPrimary;
        GUI.skin.button.hover.textColor = TextPrimary;
        GUI.skin.toggle.fontSize = 14;
        GUI.skin.toggle.normal.textColor = TextSecondary;
        GUI.skin.horizontalSlider.fontSize = 13;
        GUI.skin.textField.fontSize = 13;

        var vThumb = GUI.skin.verticalScrollbarThumb;
        vThumb.fixedWidth = 7;
        vThumb.normal.background = _scrollThumbTex;
        vThumb.hover.background = _scrollThumbTex;
        vThumb.active.background = _scrollThumbTex;
        vThumb.border = new RectOffset { left = 4, right = 4, top = 4, bottom = 4 };

        var hThumb = GUI.skin.horizontalScrollbarThumb;
        hThumb.fixedHeight = 7;
        hThumb.normal.background = _scrollThumbTex;
        hThumb.hover.background = _scrollThumbTex;
        hThumb.active.background = _scrollThumbTex;
        hThumb.border = new RectOffset { left = 4, right = 4, top = 4, bottom = 4 };

        GUI.skin.verticalScrollbar.fixedWidth = 7;
        GUI.skin.horizontalScrollbar.fixedHeight = 7;

        GUI.skin.verticalScrollbarUpButton.fixedWidth = 0;
        GUI.skin.verticalScrollbarDownButton.fixedWidth = 0;
        GUI.skin.verticalScrollbarUpButton.fixedHeight = 0;
        GUI.skin.verticalScrollbarDownButton.fixedHeight = 0;
        GUI.skin.horizontalScrollbarLeftButton.fixedWidth = 0;
        GUI.skin.horizontalScrollbarLeftButton.fixedHeight = 0;
        GUI.skin.horizontalScrollbarRightButton.fixedWidth = 0;
        GUI.skin.horizontalScrollbarRightButton.fixedHeight = 0;

        var slider = GUI.skin.horizontalSlider;
        slider.normal.background = _sliderTrackTex;
        slider.hover.background = _sliderTrackTex;
        slider.active.background = _sliderTrackTex;
        slider.border = new RectOffset { left = 5, right = 5, top = 5, bottom = 5 };
        slider.fixedHeight = 5;

        var sliderThumb = GUI.skin.horizontalSliderThumb;
        sliderThumb.normal.background = _sliderThumbTex;
        sliderThumb.hover.background = _sliderThumbTex;
        sliderThumb.active.background = _sliderThumbTex;
        sliderThumb.fixedWidth = 13;
        sliderThumb.fixedHeight = 13;
    }
}
