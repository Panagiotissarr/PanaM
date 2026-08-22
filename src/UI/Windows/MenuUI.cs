using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PanaM;

public class MenuUI : MonoBehaviour
{
    public static int windowHeight = 560;
    public static int windowWidth = 720;
    private Rect _windowRect;

    public static bool isGUIActive = false;
    private List<ITab> _tabs = new();
    private int _selectedTab;
    public static float hue; // For RGB mode
    private Vector2 _contentScroll;

    #region Search Bar Data Structures
    private struct ToggleInfo
    {
        public string tabName;
        public string fieldName;
        public string label;

        public ToggleInfo(string tab, string field, string lbl)
        {
            tabName = tab;
            fieldName = field;
            label = lbl;
        }
    }

    private List<ToggleInfo> _allToggles = new();
    private string _searchQuery = "";
    private bool _searchBarFocused = false;
    private Rect _searchFieldRect;
    #endregion

    private const int NavWidth = 128;

    private void InitializeIfNeeded()
    {
        _tabs ??= new List<ITab>();
        _allToggles ??= new List<ToggleInfo>();
        _searchQuery ??= "";

        if (_tabs.Count == 0)
        {
            _tabs.Add(new MovementTab());
            _tabs.Add(new ESPTab());
            _tabs.Add(new RolesTab());
            _tabs.Add(new ShipTab());
            _tabs.Add(new ChatTab());
            _tabs.Add(new AnimationsTab());
            _tabs.Add(new ConsoleTab());
            _tabs.Add(new HostOnlyTab());
            _tabs.Add(new PassiveTab());
            _tabs.Add(new ModesTab());
            _tabs.Add(new ConfigTab());
        }

        if (_allToggles.Count == 0)
        {
            PopulateToggles();
        }
    }

    private void Start()
    {
        InitializeIfNeeded();

        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void Update()
    {

        if (Input.GetKeyDown(Utils.StringToKeycode(PanaM.menuKeybind.Value)))
        {
            // Enable or disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            if (PanaM.menuOpenOnMouse.Value)
            {
                // Teleport the window to the mouse for immediate use
                Vector2 mousePosition = Input.mousePosition;
                _windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            }
        }

        if (CheatToggles.rgbMode)
        {
            hue += Time.deltaTime * 0.3f; // Adjust speed of color change, higher multiplier = faster
            if (hue > 1f) hue -= 1f; // Loop hue back to 0 when it exceeds 1
        }

        if (CheatToggles.stealthMode != PanaM.inStealthMode)
        {
            PanaM.inStealthMode = CheatToggles.stealthMode;

            Scene scene = SceneManager.GetActiveScene();

            if (scene.name == "MainMenu" || scene.name == "MatchMaking")
            {
                SceneManager.LoadScene(scene.name);
            }
        }

        if (CheatToggles.panicMode) Utils.Panic();

        var stamp = ModManager.Instance.ModStamp;
        if (stamp) stamp.enabled = !(PanaM.inStealthMode || PanaM.isPanicked);

        if (CheatToggles.openConfig)
        {
            Utils.OpenConfigFile();
            CheatToggles.openConfig = false;
        }

        if (CheatToggles.reloadConfig)
        {
            PanaM.Plugin.Config.Reload();
            CheatToggles.reloadConfig = false;
        }

        if (CheatToggles.saveProfile)
        {
            CheatToggles.saveProfile = false; // Disable first to avoid saving it to profile
            CheatToggles.SaveTogglesToProfile();
        }

        if (CheatToggles.loadProfile)
        {
            CheatToggles.LoadTogglesFromProfile();
            CheatToggles.loadProfile = false;
        }

        // Some cheats only work if the LocalPlayer exists, so they are turned off if it does not
        if(!Utils.isPlayer)
        {
            CheatToggles.setFakeRole = false;
            CheatToggles.setFakeAlive = false;
            CheatToggles.killAll = false;
            CheatToggles.telekillPlayer = false;
            CheatToggles.killAllCrew = false;
            CheatToggles.killAllImps = false;
            CheatToggles.teleportPlayer = false;
            CheatToggles.spectate = false;
            CheatToggles.freecam = false;
            CheatToggles.killPlayer = false;
            CheatToggles.callMeeting = false;

            if (CheatToggles.runOverload)
            {
                OverloadUI.StopOverload();
            }
        }

        // Some cheats only work if the ship exists, so they are turned off if it does not
        if(!Utils.isShip)
        {
            CheatToggles.sabotageMap = false;
            CheatToggles.unfixableLights = false;
            CheatToggles.completeMyTasks = false;
            CheatToggles.kickVents = false;
            CheatToggles.reportBody = false;
            CheatToggles.closeMeeting = false;
            CheatToggles.reactorSab = false;
            CheatToggles.oxygenSab = false;
            CheatToggles.commsSab = false;
            CheatToggles.elecSab = false;
            CheatToggles.mushSab = false;
            CheatToggles.closeAllDoors = false;
            CheatToggles.openAllDoors = false;
            CheatToggles.spamCloseAllDoors = false;
            CheatToggles.spamOpenAllDoors = false;
            CheatToggles.mushSpore = false;

            PanaMCheats.StopShipAnimCheats();
        }

        if(!Utils.isHost && !Utils.isFreePlay)
        {
            CheatToggles.killAll = false;
            CheatToggles.telekillPlayer = false;
            CheatToggles.killAllCrew = false;
            CheatToggles.killAllImps = false;
            CheatToggles.killPlayer = false;
            CheatToggles.ejectPlayer = false;
            CheatToggles.noKillCd = false;
            CheatToggles.killAnyone = false;
            CheatToggles.killVanished = false;
            CheatToggles.forceStartGame = false;
            CheatToggles.skipMeeting = false;
            CheatToggles.voteImmune = false;
            CheatToggles.noGameEnd = false;
            CheatToggles.showProtectMenu = false;
            CheatToggles.showRolesMenu = false;
            CheatToggles.noOptionsLimits = false;
        }

        // Some cheats only work if in a meeting, so they are turned off if it does not
        if (!Utils.isMeeting)
        {
            CheatToggles.skipMeeting = false;
            CheatToggles.ejectPlayer = false;
        }
    }

    public void OnGUI()
    {
        if (!isGUIActive || PanaM.isPanicked) return;

        UIHelpers.ApplyUIColor();

        Theme.ApplySkinTheme();

        _windowRect = GUI.Window((int)WindowId.MenuUI, _windowRect, (GUI.WindowFunction)WindowFunction,
            GUIContent.none, Theme.InvisibleWindowStyle);
    }

    public void WindowFunction(int windowID)
    {
        InitializeIfNeeded();

        var windowRect = new Rect(0, 0, _windowRect.width, _windowRect.height);
        Event e = Event.current;

        Theme.DrawWindowChrome(windowRect);

        DrawTitleBar(e);

        GUILayout.Space(Theme.TitleBarHeight - 4);

        GUILayout.BeginHorizontal();
        GUILayout.Space(14);
        DrawSearchField(e);
        GUILayout.Space(14);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(14);

        List<ITab> visibleTabs = ComputeVisibleTabs();

        ITab activeTab = ResolveActiveTab(visibleTabs);

        DrawSidebar(visibleTabs, activeTab);

        GUILayout.Space(12);

        var dividerRect = GUILayoutUtility.GetRect(1, 1, GUILayout.Width(1), GUILayout.ExpandHeight(true));
        Theme.DrawRect(dividerRect, Theme.DividerColor);

        GUILayout.Space(12);

        DrawContent(activeTab, visibleTabs, e);

        GUILayout.Space(14);
        GUILayout.EndHorizontal();

        GUI.DragWindow(new Rect(0, 0, windowWidth, Theme.TitleBarHeight));
    }

    private void DrawTitleBar(Event e)
    {
        float w = _windowRect.width;

        GUI.Label(new Rect(18, 9, 140, 26), "PanaM", Theme.TitleStyle);
        Theme.DrawCircle(new Rect(92, 18, 8, 8), Theme.Accent);

        string versionStr = "v" + PanaM.panamVersion;
        var badgeRect = new Rect(w - 74, 12, 58, 18);
        Theme.DrawRounded(badgeRect, 9, Theme.AccentSoft);

        var badgeStyle = Theme.MutedStyle;
        badgeStyle.alignment = TextAnchor.MiddleCenter;
        badgeStyle.normal.textColor = Color.Lerp(Theme.Accent, Color.white, 0.6f);
        GUI.Label(badgeRect, versionStr, badgeStyle);
        badgeStyle.alignment = TextAnchor.MiddleLeft;
        badgeStyle.normal.textColor = Theme.TextMuted;
    }

    private void DrawSearchField(Event e)
    {
        var fieldRect = GUILayoutUtility.GetRect(0, Theme.SearchFieldHeight, GUILayout.ExpandWidth(true));
        _searchFieldRect = fieldRect;

        bool focused = _searchBarFocused;

        if (focused)
        {
            Theme.DrawRounded(fieldRect, 9, new Color(Theme.Accent.r, Theme.Accent.g, Theme.Accent.b, 0.55f));
            Theme.DrawRounded(new Rect(fieldRect.x + 1, fieldRect.y + 1, fieldRect.width - 2, fieldRect.height - 2),
                8, new Color(1f, 1f, 1f, 0.08f));
        }
        else
        {
            Theme.DrawRounded(fieldRect, 9, new Color(1f, 1f, 1f, 0.05f));
        }

        string displayText;
        if (focused)
        {
            displayText = _searchQuery + (System.DateTime.Now.Millisecond / 500 % 2 == 0 ? "|" : "");
        }
        else if (string.IsNullOrEmpty(_searchQuery))
        {
            displayText = "Search cheats...";
        }
        else
        {
            displayText = _searchQuery;
        }

        var textStyle = Theme.BodyStyle;
        textStyle.normal.textColor = focused ? Theme.TextPrimary :
            string.IsNullOrEmpty(_searchQuery) ? Theme.TextMuted : Theme.TextSecondary;

        float clearWidth = string.IsNullOrEmpty(_searchQuery) ? 0 : 24;
        var textRect = new Rect(fieldRect.x + 12, fieldRect.y + 4, fieldRect.width - clearWidth - 20, fieldRect.height - 8);
        GUI.Label(textRect, displayText, textStyle);

        if (clearWidth > 0)
        {
            var clearRect = new Rect(fieldRect.xMax - clearWidth - 4, fieldRect.y + (fieldRect.height - 20) / 2f, 20, 20);
            if (clearRect.Contains(e.mousePosition))
            {
                Theme.DrawRounded(clearRect, 6, Theme.SurfaceHover);
            }

            var xStyle = Theme.MutedStyle;
            xStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(clearRect, "\u2715", xStyle);
            xStyle.alignment = TextAnchor.MiddleLeft;

            if (e.type == EventType.MouseDown && e.button == 0 && clearRect.Contains(e.mousePosition))
            {
                _searchQuery = "";
                _searchBarFocused = false;
                e.Use();
            }
        }

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (fieldRect.Contains(e.mousePosition))
            {
                _searchBarFocused = true;
                e.Use();
            }
            else
            {
                _searchBarFocused = false;
            }
        }

        if (!_searchBarFocused) return;

        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Backspace)
            {
                if (_searchQuery.Length > 0)
                {
                    _searchQuery = _searchQuery.Substring(0, _searchQuery.Length - 1);
                }
                e.Use();
            }
            else if (e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.Return)
            {
                _searchBarFocused = false;
                e.Use();
            }
            else if (e.character != '\0' && e.character != '\n' && e.character != '\r' && e.character != '\t' && e.character != '\b')
            {
                _searchQuery += e.character;
                e.Use();
            }
        }
    }

    private bool TabHasMatches(string tabName, string query)
    {
        foreach (var toggle in _allToggles)
        {
            if (toggle.tabName.Equals(tabName, System.StringComparison.OrdinalIgnoreCase) &&
                toggle.label.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private List<ITab> ComputeVisibleTabs()
    {
        if (string.IsNullOrWhiteSpace(_searchQuery)) return _tabs;

        var query = _searchQuery.Trim();
        var result = new List<ITab>();

        foreach (var tab in _tabs)
        {
            if (TabHasMatches(tab.name, query))
            {
                result.Add(tab);
            }
        }

        return result;
    }

    private ITab ResolveActiveTab(List<ITab> visibleTabs)
    {
        if (visibleTabs.Count == 0) return null;

        if (_selectedTab >= 0 && _selectedTab < _tabs.Count)
        {
            var curTab = _tabs[_selectedTab];
            if (visibleTabs.Contains(curTab)) return curTab;
        }

        var fallback = visibleTabs[0];
        _selectedTab = _tabs.IndexOf(fallback);
        return fallback;
    }

    private void DrawSidebar(List<ITab> visibleTabs, ITab activeTab)
    {
        GUILayout.BeginVertical(GUILayout.Width(NavWidth));

        foreach (var tab in visibleTabs)
        {
            bool selected = tab == activeTab;
            var itemRect = GUILayoutUtility.GetRect(NavWidth - 6, 30, GUILayout.ExpandWidth(false));

            var e = Event.current;
            bool hover = !selected && itemRect.Contains(e.mousePosition);

            if (selected)
            {
                Theme.DrawRounded(itemRect, 7, Theme.AccentSoft);
                var barRect = new Rect(itemRect.x, itemRect.y + 8, 3, itemRect.height - 16);
                Theme.DrawRect(barRect, Theme.Accent);
            }
            else if (hover)
            {
                Theme.DrawRounded(itemRect, 7, Theme.SurfaceIdle);
            }

            var labelStyle = Theme.BodyStyle;
            labelStyle.fontSize = 13;
            labelStyle.normal.textColor = selected ? Color.white : hover ? Theme.TextPrimary : Theme.TextSecondary;
            GUI.Label(new Rect(itemRect.x + 12, itemRect.y + 4, itemRect.width - 16, itemRect.height - 8),
                tab.name, labelStyle);
            labelStyle.fontSize = 14;
            labelStyle.normal.textColor = Theme.TextPrimary;

            if (e.type == EventType.MouseDown && e.button == 0 && itemRect.Contains(e.mousePosition))
            {
                _selectedTab = _tabs.IndexOf(tab);
                _searchBarFocused = false;
                _contentScroll = Vector2.zero;
                e.Use();
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    private void DrawContent(ITab activeTab, List<ITab> visibleTabs, Event e)
    {
        GUILayout.BeginVertical();

        if (activeTab != null)
        {
            GUI.Label(GUILayoutUtility.GetRect(0, 26, GUILayout.ExpandWidth(true)), activeTab.name.ToUpperInvariant(), Theme.SectionStyle);
            GUILayout.Space(2);

            _contentScroll = GUILayout.BeginScrollView(_contentScroll, false, true,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                activeTab.Draw();
            }
            else
            {
                var query = _searchQuery.Trim();

                foreach (var tab in visibleTabs)
                {
                    var headerRect = GUILayoutUtility.GetRect(0, 24, GUILayout.ExpandWidth(true));
                    bool hover = headerRect.Contains(e.mousePosition);
                    if (hover) Theme.DrawRounded(headerRect, 6, Theme.SurfaceIdle);

                    var headerStyle = Theme.BodyStyle;
                    headerStyle.fontStyle = FontStyle.Bold;
                    headerStyle.normal.textColor = hover ? Theme.Accent : Color.Lerp(Theme.Accent, Color.white, 0.4f);
                    GUI.Label(new Rect(headerRect.x, headerRect.y + 2, headerRect.width, headerRect.height),
                        tab.name, headerStyle);
                    headerStyle.fontStyle = FontStyle.Normal;
                    headerStyle.normal.textColor = Theme.TextPrimary;

                    if (e.type == EventType.MouseDown && e.button == 0 && headerRect.Contains(e.mousePosition))
                    {
                        _selectedTab = _tabs.IndexOf(tab);
                        _searchQuery = "";
                        _searchBarFocused = false;
                        _contentScroll = Vector2.zero;
                        e.Use();
                    }

                    foreach (var toggle in _allToggles)
                    {
                        if (toggle.tabName.Equals(tab.name, System.StringComparison.OrdinalIgnoreCase) &&
                            toggle.label.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            DrawSearchToggle(toggle.fieldName, toggle.label);
                        }
                    }

                    GUILayout.Space(6);
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No matching cheats", Theme.MutedStyle);
            GUILayout.FlexibleSpace();
        }

        GUILayout.EndVertical();
    }

    #region Search Bar Helpers
    private void DrawSearchToggle(string fieldName, string label)
    {
        if (CheatToggles.ToggleFields.TryGetValue(fieldName, out var field))
        {
            bool val = (bool)field.GetValue(null);
            bool newVal = Widgets.Toggle(val, label);
            if (newVal != val)
            {
                field.SetValue(null, newVal);
            }
        }
    }

    private void PopulateToggles()
    {
        _allToggles.Add(new("Movement", "noClip", "NoClip"));
        _allToggles.Add(new("Movement", "invertControls", "Invert Controls"));
        _allToggles.Add(new("Movement", "teleportCursor", "to Cursor"));
        _allToggles.Add(new("Movement", "teleportPlayer", "to Player"));

        _allToggles.Add(new("ESP", "seePlayerInfo", "See Player Info"));
        _allToggles.Add(new("ESP", "seeRoles", "See Roles"));
        _allToggles.Add(new("ESP", "seeGhosts", "See Ghosts"));
        _allToggles.Add(new("ESP", "noShadows", "No Shadows"));
        _allToggles.Add(new("ESP", "taskArrows", "Task Arrows"));
        _allToggles.Add(new("ESP", "revealVotes", "Reveal Votes"));
        _allToggles.Add(new("ESP", "seeLobbyInfo", "See Lobby Info"));
        _allToggles.Add(new("ESP", "zoomOut", "Zoom Out"));
        _allToggles.Add(new("ESP", "spectate", "Spectate"));
        _allToggles.Add(new("ESP", "freecam", "Freecam"));
        _allToggles.Add(new("ESP", "tracersCrew", "Tracers: Crewmates"));
        _allToggles.Add(new("ESP", "tracersImps", "Tracers: Impostors"));
        _allToggles.Add(new("ESP", "tracersGhosts", "Tracers: Ghosts"));
        _allToggles.Add(new("ESP", "tracersBodies", "Tracers: Dead Bodies"));
        _allToggles.Add(new("ESP", "colorBasedTracers", "Tracers: Color-based"));
        _allToggles.Add(new("ESP", "distanceBasedTracers", "Tracers: Distance-based"));
        _allToggles.Add(new("ESP", "mapCrew", "Minimap: Crewmates"));
        _allToggles.Add(new("ESP", "mapImps", "Minimap: Impostors"));
        _allToggles.Add(new("ESP", "mapGhosts", "Minimap: Ghosts"));
        _allToggles.Add(new("ESP", "colorBasedMap", "Minimap: Color-based"));

        _allToggles.Add(new("Roles", "setFakeRole", "Set Fake Role"));
        _allToggles.Add(new("Roles", "setFakeAlive", "Set Fake Alive"));
        _allToggles.Add(new("Roles", "killReach", "Impostor: Kill Reach"));
        _allToggles.Add(new("Roles", "noShapeshiftAnim", "Shapeshifter: No Ss Animation"));
        _allToggles.Add(new("Roles", "endlessSsDuration", "Shapeshifter: Endless Ss Duration"));
        _allToggles.Add(new("Roles", "showTasksMenu", "Crewmate: Show Tasks Menu"));
        _allToggles.Add(new("Roles", "endlessTracking", "Tracker: Endless Tracking"));
        _allToggles.Add(new("Roles", "noTrackingDelay", "Tracker: No Track Delay"));
        _allToggles.Add(new("Roles", "noTrackingCooldown", "Tracker: No Track Cooldown"));
        _allToggles.Add(new("Roles", "trackReach", "Tracker: Track Reach"));
        _allToggles.Add(new("Roles", "endlessVentTime", "Engineer: Endless Vent Time"));
        _allToggles.Add(new("Roles", "noVentCooldown", "Engineer: No Vent Cooldown"));
        _allToggles.Add(new("Roles", "endlessBattery", "Scientist: Endless Battery"));
        _allToggles.Add(new("Roles", "noVitalsCooldown", "Scientist: No Vitals Cooldown"));
        _allToggles.Add(new("Roles", "interrogateReach", "Detective: Interrogate Reach"));

        _allToggles.Add(new("Ship", "unfixableLights", "Unfixable Lights"));
        _allToggles.Add(new("Ship", "callMeeting", "Call Meeting"));
        _allToggles.Add(new("Ship", "closeMeeting", "Close Meeting"));
        _allToggles.Add(new("Ship", "autoReportBodies", "Auto-Report Dead Bodies"));
        _allToggles.Add(new("Ship", "autoOpenDoorsOnUse", "Auto-Open Doors On Use"));
        _allToggles.Add(new("Ship", "reactorSab", "Sabotage: Reactor"));
        _allToggles.Add(new("Ship", "oxygenSab", "Sabotage: Oxygen"));
        _allToggles.Add(new("Ship", "elecSab", "Sabotage: Lights"));
        _allToggles.Add(new("Ship", "commsSab", "Sabotage: Comms"));
        _allToggles.Add(new("Ship", "showDoorsMenu", "Sabotage: Show Doors Menu"));
        _allToggles.Add(new("Ship", "mushSab", "Sabotage: Mushroom Mixup"));
        _allToggles.Add(new("Ship", "mushSpore", "Sabotage: Trigger Spores"));
        _allToggles.Add(new("Ship", "sabotageMap", "Sabotage: Open Sabotage Map"));
        _allToggles.Add(new("Ship", "unlockVents", "Vents: Unlock Vents"));
        _allToggles.Add(new("Ship", "kickVents", "Vents: Kick All From Vents"));
        _allToggles.Add(new("Ship", "walkInVents", "Vents: Walk In Vents"));

        _allToggles.Add(new("Chat", "enableChat", "Enable Chat"));
        _allToggles.Add(new("Chat", "bypassUrlBlock", "Bypass URL Block"));
        _allToggles.Add(new("Chat", "lowerRateLimits", "Lower Rate Limits"));
        _allToggles.Add(new("Chat", "unlockCharacters", "Textbox: Unlock Extra Characters"));
        _allToggles.Add(new("Chat", "longerMessages", "Textbox: Allow Longer Messages"));
        _allToggles.Add(new("Chat", "unlockClipboard", "Textbox: Unlock Clipboard"));

        _allToggles.Add(new("Animations", "animShields", "Shields"));
        _allToggles.Add(new("Animations", "animAsteroids", "Asteroids"));
        _allToggles.Add(new("Animations", "animEmptyGarbage", "Empty Garbage"));
        _allToggles.Add(new("Animations", "animMedScan", "Medbay Scan"));
        _allToggles.Add(new("Animations", "animCamsInUse", "Cams In Use"));
        _allToggles.Add(new("Animations", "moonWalk", "Client-Sided: Moonwalk"));

        _allToggles.Add(new("Console", "showConsole", "Show Console"));
        _allToggles.Add(new("Console", "logDeaths", "Log Deaths"));
        _allToggles.Add(new("Console", "logShapeshifts", "Log Shapeshifts"));
        _allToggles.Add(new("Console", "logVents", "Log Vents"));

        _allToggles.Add(new("Host-Only", "killVanished", "Kill While Vanished"));
        _allToggles.Add(new("Host-Only", "killAnyone", "Kill Anyone"));
        _allToggles.Add(new("Host-Only", "noKillCd", "No Kill Cooldown"));
        _allToggles.Add(new("Host-Only", "showProtectMenu", "Show Protect Menu"));
        _allToggles.Add(new("Host-Only", "killPlayer", "Murder: Kill Player"));
        _allToggles.Add(new("Host-Only", "telekillPlayer", "Murder: Telekill Player"));
        _allToggles.Add(new("Host-Only", "killAllCrew", "Murder: Kill All Crewmates"));
        _allToggles.Add(new("Host-Only", "killAllImps", "Murder: Kill All Impostors"));
        _allToggles.Add(new("Host-Only", "killAll", "Murder: Kill Everyone"));
        _allToggles.Add(new("Host-Only", "forceStartGame", "Game State: Force Start Game"));
        _allToggles.Add(new("Host-Only", "noGameEnd", "Game State: No Game End"));
        _allToggles.Add(new("Host-Only", "skipMeeting", "Meetings: Skip Meeting"));
        _allToggles.Add(new("Host-Only", "voteImmune", "Meetings: Vote Immune"));
        _allToggles.Add(new("Host-Only", "ejectPlayer", "Meetings: Eject Player"));

        _allToggles.Add(new("Passive", "unlockFeatures", "Unlock Extra Features"));
        _allToggles.Add(new("Passive", "freeCosmetics", "Free Cosmetics"));
        _allToggles.Add(new("Passive", "avoidPenalties", "Avoid Penalties"));
        _allToggles.Add(new("Passive", "copyLobbyCodeOnDisconnect", "Copy Lobby Code on Disconnect"));
        _allToggles.Add(new("Passive", "spoofAprilFoolsDate", "Spoof Date to April 1st"));

        _allToggles.Add(new("Modes", "rgbMode", "RGB Mode"));
        _allToggles.Add(new("Modes", "stealthMode", "Stealth Mode"));
        _allToggles.Add(new("Modes", "panicMode", "Panic Mode"));

        _allToggles.Add(new("Config", "openConfig", "Open Config"));
        _allToggles.Add(new("Config", "reloadConfig", "Reload Config"));
        _allToggles.Add(new("Config", "saveProfile", "Save to Profile"));
        _allToggles.Add(new("Config", "loadProfile", "Load from Profile"));

        _allToggles.Add(new("Overload", "showOverload", "Show Overload Menu"));
        _allToggles.Add(new("Overload", "showOverloadSettings", "Show Overload Settings"));
        _allToggles.Add(new("Overload", "olAutoAdapt", "Auto Adapt"));
        _allToggles.Add(new("Overload", "olShowRpcTotal", "RPC Total"));
        _allToggles.Add(new("Overload", "olAutoStart", "Auto Start when Ready"));
        _allToggles.Add(new("Overload", "olAutoStop", "Auto Stop when Done"));
        _allToggles.Add(new("Overload", "olLockTargets", "Lock Targets on Start"));
        _allToggles.Add(new("Overload", "olKillSwitch", "Kill Switch on Lag"));
        _allToggles.Add(new("Overload", "olLogStartStop", "Log START and STOP"));
        _allToggles.Add(new("Overload", "olLogAddRemove", "Log ADD and REMOVE"));
        _allToggles.Add(new("Overload", "olLogAttack", "Log Attack"));
        _allToggles.Add(new("Overload", "olLogDisconnect", "Log Disconnect"));
        _allToggles.Add(new("Overload", "olVerboseLogs", "Verbose Attack Logs"));
        _allToggles.Add(new("Overload", "olAutoClear", "Auto Clear on Start"));
    }
    #endregion
}
