using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace PanaM;

public class MenuUI : MonoBehaviour
{
    public static int windowHeight = 550;
    public static int windowWidth = 700;
    private Rect _windowRect;

    public static bool isGUIActive = false;
    private List<ITab> _tabs = new();
    private int _selectedTab;
    public static float hue; // For RGB mode

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
    #endregion

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

    public void InitStyles()
    {
        GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = GUI.skin.label.fontSize = 15;
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

        InitStyles();

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.MenuUI, _windowRect, (GUI.WindowFunction)WindowFunction, "PanaM v" + PanaM.panamVersion);
    }

    public void WindowFunction(int windowID)
    {
        InitializeIfNeeded();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(60));

        string displayText = string.IsNullOrEmpty(_searchQuery) ? "Type here..." : _searchQuery;
        if (_searchBarFocused)
        {
            displayText = _searchQuery + (System.DateTime.Now.Millisecond / 500 % 2 == 0 ? "|" : "");
        }

        GUIStyle searchBoxStyle = new GUIStyle(GUI.skin.box);
        if (_searchBarFocused)
        {
            searchBoxStyle.normal.textColor = Color.white;
        }
        else
        {
            searchBoxStyle.normal.textColor = Color.gray;
        }

        if (GUILayout.Button(displayText, searchBoxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(25)))
        {
            _searchBarFocused = true;
        }
        Rect searchRect = GUILayoutUtility.GetLastRect();

        if (GUILayout.Button("Clear", GUILayout.Width(60)))
        {
            _searchQuery = "";
            _searchBarFocused = false;
        }
        GUILayout.EndHorizontal();

        Event e = Event.current;
        if (e != null)
        {
            if (e.type == EventType.MouseDown)
            {
                if (!searchRect.Contains(e.mousePosition))
                {
                    _searchBarFocused = false;
                }
            }
            else if (_searchBarFocused && e.type == EventType.KeyDown)
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

        GUILayout.Space(10);

        List<ITab> visibleTabs = new();
        if (string.IsNullOrWhiteSpace(_searchQuery))
        {
            visibleTabs = _tabs;
        }
        else
        {
            var query = _searchQuery.Trim();
            foreach (var tab in _tabs)
            {
                bool hasMatch = false;
                foreach (var toggle in _allToggles)
                {
                    if (toggle.tabName.Equals(tab.name, System.StringComparison.OrdinalIgnoreCase) &&
                        toggle.label.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasMatch = true;
                        break;
                    }
                }
                if (hasMatch)
                {
                    visibleTabs.Add(tab);
                }
            }
        }

        ITab activeTab = null;
        if (visibleTabs.Count > 0)
        {
            if (_selectedTab >= 0 && _selectedTab < _tabs.Count)
            {
                var curTab = _tabs[_selectedTab];
                if (visibleTabs.Contains(curTab))
                {
                    activeTab = curTab;
                }
                else
                {
                    activeTab = visibleTabs[0];
                    _selectedTab = _tabs.IndexOf(activeTab);
                }
            }
            else
            {
                activeTab = visibleTabs[0];
                _selectedTab = _tabs.IndexOf(activeTab);
            }
        }

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(windowWidth * 0.15f));
        for (var i = 0; i < visibleTabs.Count; i++)
        {
            Color standardColor = GUI.backgroundColor;

            if (activeTab == visibleTabs[i])
            {
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            }

            if (GUILayout.Button(visibleTabs[i].name, GUIStylePreset.TabButton, GUILayout.Height(35)))
            {
                activeTab = visibleTabs[i];
                _selectedTab = _tabs.IndexOf(activeTab);
            }

            GUI.backgroundColor = standardColor;
        }
        GUILayout.EndVertical();

        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Width(1f), GUILayout.ExpandHeight(true));
        GUILayout.Space(10f);

        GUILayout.BeginVertical(GUILayout.Width(windowWidth * 0.85f));

        if (activeTab != null)
        {
            GUILayout.Label(activeTab.name, GUIStylePreset.TabTitle);

            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                activeTab.Draw();
            }
            else
            {
                var query = _searchQuery.Trim();
                GUILayout.BeginVertical();
                foreach (var toggle in _allToggles)
                {
                    if (toggle.tabName.Equals(activeTab.name, System.StringComparison.OrdinalIgnoreCase) &&
                        toggle.label.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        DrawSearchToggle(toggle.fieldName, toggle.label);
                    }
                }
                GUILayout.EndVertical();
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUI.DragWindow(new Rect(0, 0, windowWidth, 25));
    }

    #region Search Bar Helpers
    private void DrawSearchToggle(string fieldName, string label)
    {
        if (CheatToggles.ToggleFields.TryGetValue(fieldName, out var field))
        {
            bool val = (bool)field.GetValue(null);
            bool newVal = GUILayout.Toggle(val, " " + label);
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
