using UnityEngine;
using Il2CppInterop.Runtime.Injection;

namespace PanaM;

public class BackdropBlur : MonoBehaviour
{
    private static BackdropBlur _instance;
    private static BlurHook _hook;
    private static Camera _hookedCamera;
    private static RenderTexture[] _chain;
    private static RenderTexture _latest;
    private static float _lastHookFrameTime = -999f;
    private static int _lastScreenWidth;
    private static int _lastScreenHeight;

    public static bool HasFrame => _latest != null && Time.unscaledTime - _lastHookFrameTime < 0.5f;
    public static bool Active => MenuUI.isGUIActive && !PanaM.isPanicked && Theme.BlurEnabled;
    public static BackdropBlur Instance => _instance;

    public static void Create()
    {
        if (_instance != null) return;

        ClassInjector.RegisterTypeInIl2Cpp<BackdropBlur>();
        ClassInjector.RegisterTypeInIl2Cpp<BlurHook>();

        var go = new GameObject("PanaM_BackdropBlur");
        go.hideFlags = HideFlags.HideAndDontSave;
        _instance = go.AddComponent<BackdropBlur>();
    }

    public static void DestroyInstance()
    {
        if (_instance == null) return;

        if (_hook != null) UnityEngine.Object.Destroy(_hook);
        UnityEngine.Object.Destroy(_instance.gameObject);
        _instance = null;
        _hook = null;
        _hookedCamera = null;
        ReleaseChain();
    }

    private void Update()
    {
        if (!Active)
        {
            ReleaseChain();
            return;
        }

        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            ReleaseChain();
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }

        if (Camera.main != _hookedCamera || _hook == null)
        {
            Rebind();
        }
    }

    private void Rebind()
    {
        var cam = Camera.main;

        if (_hook != null && _hookedCamera != null && cam != _hookedCamera)
        {
            UnityEngine.Object.Destroy(_hook);
            _hook = null;
            _hookedCamera = null;
            ReleaseChain();
        }

        if (cam == null) return;

        _hook = cam.gameObject.GetComponent<BlurHook>();
        if (_hook == null)
        {
            _hook = cam.gameObject.AddComponent<BlurHook>();
        }

        _hookedCamera = cam;
    }

    internal static void Process(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);

        try
        {
            if (!MenuUI.isGUIActive || PanaM.isPanicked)
            {
                _lastHookFrameTime = -999f;
                return;
            }

            EnsureChain(source);

            var current = source;
            foreach (var rt in _chain)
            {
                Graphics.Blit(current, rt);
                current = rt;
            }

            _latest = current;
            _lastHookFrameTime = Time.unscaledTime;
        }
        catch
        {
            _latest = null;
        }
    }

    private static void EnsureChain(RenderTexture source)
    {
        int steps = MaxSteps();

        if (_chain != null && _chain.Length == steps && _chain[0] != null && _chain[steps - 1].width == Mathf.Max(8, Screen.width / (1 << (steps + 1)))) return;

        ReleaseChain();
        _chain = new RenderTexture[steps];

        for (int i = 0; i < steps; i++)
        {
            int div = 1 << (i + 2);
            int w = Mathf.Max(8, Screen.width / div);
            int h = Mathf.Max(8, Screen.height / div);
            _chain[i] = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
        }
    }

    private static int MaxSteps()
    {
        return 3;
    }

    private static void ReleaseChain()
    {
        if (_chain == null) return;

        foreach (var rt in _chain)
        {
            if (rt != null) rt.Release();
        }

        _chain = null;
        _latest = null;
    }

    public static void DrawRegion(Rect rect)
    {
        if (_latest == null) return;

        float sw = Screen.width;
        float sh = Screen.height;

        var uv = new Rect(
            rect.x / sw,
            1f - (rect.y + rect.height) / sh,
            rect.width / sw,
            rect.height / sh);

        GUI.DrawTextureWithTexCoords(rect, _latest, uv, true);
    }
}

public class BlurHook : MonoBehaviour
{
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        BackdropBlur.Process(source, destination);
    }
}
