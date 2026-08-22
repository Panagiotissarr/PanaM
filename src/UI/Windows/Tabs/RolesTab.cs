using UnityEngine;

namespace PanaM;

public class RolesTab : ITab
{
    public string name => "Roles";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.setFakeRole = Widgets.Toggle(CheatToggles.setFakeRole, "Set Fake Role");

        CheatToggles.setFakeAlive = Widgets.Toggle(CheatToggles.setFakeAlive, "Set Fake Alive");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Impostor");

        CheatToggles.killReach = Widgets.Toggle(CheatToggles.killReach, "Kill Reach");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Shapeshifter");

        CheatToggles.noShapeshiftAnim = Widgets.Toggle(CheatToggles.noShapeshiftAnim, "No Ss Animation");

        CheatToggles.endlessSsDuration = Widgets.Toggle(CheatToggles.endlessSsDuration, "Endless Ss Duration");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Crewmate");

        CheatToggles.showTasksMenu = Widgets.Toggle(CheatToggles.showTasksMenu, "Show Tasks Menu");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Tracker");

        CheatToggles.endlessTracking = Widgets.Toggle(CheatToggles.endlessTracking, "Endless Tracking");

        CheatToggles.noTrackingDelay = Widgets.Toggle(CheatToggles.noTrackingDelay, "No Track Delay");

        CheatToggles.noTrackingCooldown = Widgets.Toggle(CheatToggles.noTrackingCooldown, "No Track Cooldown");

        CheatToggles.trackReach = Widgets.Toggle(CheatToggles.trackReach, "Track Reach");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        Widgets.BeginSection("Engineer");

        CheatToggles.endlessVentTime = Widgets.Toggle(CheatToggles.endlessVentTime, "Endless Vent Time");

        CheatToggles.noVentCooldown = Widgets.Toggle(CheatToggles.noVentCooldown, "No Vent Cooldown");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Scientist");

        CheatToggles.endlessBattery = Widgets.Toggle(CheatToggles.endlessBattery, "Endless Battery");

        CheatToggles.noVitalsCooldown = Widgets.Toggle(CheatToggles.noVitalsCooldown, "No Vitals Cooldown");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Detective");

        CheatToggles.interrogateReach = Widgets.Toggle(CheatToggles.interrogateReach, "Interrogate Reach");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
