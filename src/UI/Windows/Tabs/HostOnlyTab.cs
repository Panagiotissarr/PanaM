using UnityEngine;

namespace PanaM;

public class HostOnlyTab : ITab
{
    public string name => "Host-Only";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.killVanished = Widgets.Toggle(CheatToggles.killVanished, "Kill While Vanished");

        CheatToggles.killAnyone = Widgets.Toggle(CheatToggles.killAnyone, "Kill Anyone");

        CheatToggles.noKillCd = Widgets.Toggle(CheatToggles.noKillCd, "No Kill Cooldown");

        CheatToggles.showProtectMenu = Widgets.Toggle(CheatToggles.showProtectMenu, "Show Protect Menu");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Murder");

        CheatToggles.killPlayer = Widgets.Toggle(CheatToggles.killPlayer, "Kill Player");

        CheatToggles.telekillPlayer = Widgets.Toggle(CheatToggles.telekillPlayer, "Telekill Player");

        CheatToggles.killAllCrew = Widgets.Toggle(CheatToggles.killAllCrew, "Kill All Crewmates");

        CheatToggles.killAllImps = Widgets.Toggle(CheatToggles.killAllImps, "Kill All Impostors");

        CheatToggles.killAll = Widgets.Toggle(CheatToggles.killAll, "Kill Everyone");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Game State");

        CheatToggles.forceStartGame = Widgets.Toggle(CheatToggles.forceStartGame, "Force Start Game");

        CheatToggles.noGameEnd = Widgets.Toggle(CheatToggles.noGameEnd, "No Game End");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        Widgets.BeginSection("Meetings");

        CheatToggles.skipMeeting = Widgets.Toggle(CheatToggles.skipMeeting, "Skip Meeting");

        CheatToggles.voteImmune = Widgets.Toggle(CheatToggles.voteImmune, "Vote Immune");

        CheatToggles.ejectPlayer = Widgets.Toggle(CheatToggles.ejectPlayer, "Eject Player");

        Widgets.EndSection();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}
