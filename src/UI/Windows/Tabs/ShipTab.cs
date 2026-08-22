using UnityEngine;

namespace PanaM;

public class ShipTab : ITab
{
    public string name => "Ship";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.unfixableLights = Widgets.Toggle(CheatToggles.unfixableLights, "Unfixable Lights");

        CheatToggles.callMeeting = Widgets.Toggle(CheatToggles.callMeeting, "Call Meeting");

        CheatToggles.closeMeeting = Widgets.Toggle(CheatToggles.closeMeeting, "Close Meeting");

        CheatToggles.autoReportBodies = Widgets.Toggle(CheatToggles.autoReportBodies, "Auto-Report Dead Bodies");

        CheatToggles.autoOpenDoorsOnUse = Widgets.Toggle(CheatToggles.autoOpenDoorsOnUse, "Auto-Open Doors On Use");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Sabotage");

        CheatToggles.reactorSab = Widgets.Toggle(CheatToggles.reactorSab, "Reactor");

        CheatToggles.oxygenSab = Widgets.Toggle(CheatToggles.oxygenSab, "Oxygen");

        CheatToggles.elecSab = Widgets.Toggle(CheatToggles.elecSab, "Lights");

        CheatToggles.commsSab = Widgets.Toggle(CheatToggles.commsSab, "Comms");

        CheatToggles.showDoorsMenu = Widgets.Toggle(CheatToggles.showDoorsMenu, "Show Doors Menu");

        CheatToggles.mushSab = Widgets.Toggle(CheatToggles.mushSab, "Mushroom Mixup");

        CheatToggles.mushSpore = Widgets.Toggle(CheatToggles.mushSpore, "Trigger Spores");

        CheatToggles.sabotageMap = Widgets.Toggle(CheatToggles.sabotageMap, "Open Sabotage Map");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        Widgets.BeginSection("Vents");

        CheatToggles.unlockVents = Widgets.Toggle(CheatToggles.unlockVents, "Unlock Vents");

        CheatToggles.kickVents = Widgets.Toggle(CheatToggles.kickVents, "Kick All From Vents");

        CheatToggles.walkInVents = Widgets.Toggle(CheatToggles.walkInVents, "Walk In Vents");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
