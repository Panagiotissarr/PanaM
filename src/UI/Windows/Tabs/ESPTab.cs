using UnityEngine;

namespace PanaM;

public class ESPTab : ITab
{
    public string name => "ESP";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.seePlayerInfo = Widgets.Toggle(CheatToggles.seePlayerInfo, "See Player Info");

        CheatToggles.seeRoles = Widgets.Toggle(CheatToggles.seeRoles, "See Roles");

        CheatToggles.seeGhosts = Widgets.Toggle(CheatToggles.seeGhosts, "See Ghosts");

        CheatToggles.noShadows = Widgets.Toggle(CheatToggles.noShadows, "No Shadows");

        CheatToggles.taskArrows = Widgets.Toggle(CheatToggles.taskArrows, "Task Arrows");

        CheatToggles.revealVotes = Widgets.Toggle(CheatToggles.revealVotes, "Reveal Votes");

        CheatToggles.seeLobbyInfo = Widgets.Toggle(CheatToggles.seeLobbyInfo, "See Lobby Info");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Camera");

        CheatToggles.zoomOut = Widgets.Toggle(CheatToggles.zoomOut, "Zoom Out");

        CheatToggles.spectate = Widgets.Toggle(CheatToggles.spectate, "Spectate");

        CheatToggles.freecam = Widgets.Toggle(CheatToggles.freecam, "Freecam");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        Widgets.BeginSection("Tracers");

        CheatToggles.tracersCrew = Widgets.Toggle(CheatToggles.tracersCrew, "Crewmates");

        CheatToggles.tracersImps = Widgets.Toggle(CheatToggles.tracersImps, "Impostors");

        CheatToggles.tracersGhosts = Widgets.Toggle(CheatToggles.tracersGhosts, "Ghosts");

        CheatToggles.tracersBodies = Widgets.Toggle(CheatToggles.tracersBodies, "Dead Bodies");

        CheatToggles.colorBasedTracers = Widgets.Toggle(CheatToggles.colorBasedTracers, "Color-based");

        CheatToggles.distanceBasedTracers = Widgets.Toggle(CheatToggles.distanceBasedTracers, "Distance-based");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Minimap");

        CheatToggles.mapCrew = Widgets.Toggle(CheatToggles.mapCrew, "Crewmates");

        CheatToggles.mapImps = Widgets.Toggle(CheatToggles.mapImps, "Impostors");

        CheatToggles.mapGhosts = Widgets.Toggle(CheatToggles.mapGhosts, "Ghosts");

        CheatToggles.colorBasedMap = Widgets.Toggle(CheatToggles.colorBasedMap, "Color-based");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
