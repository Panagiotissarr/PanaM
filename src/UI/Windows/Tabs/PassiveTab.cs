using UnityEngine;

namespace PanaM;

public class PassiveTab : ITab
{
    public string name => "Passive";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.freeCosmetics = Widgets.Toggle(CheatToggles.freeCosmetics, "Free Cosmetics");

        CheatToggles.avoidPenalties = Widgets.Toggle(CheatToggles.avoidPenalties, "Avoid Penalties");

        CheatToggles.unlockFeatures = Widgets.Toggle(CheatToggles.unlockFeatures, "Unlock Extra Features");

        CheatToggles.copyLobbyCodeOnDisconnect = Widgets.Toggle(CheatToggles.copyLobbyCodeOnDisconnect, "Copy Lobby Code on Disconnect");

        CheatToggles.spoofAprilFoolsDate = Widgets.Toggle(CheatToggles.spoofAprilFoolsDate, "Spoof Date to April 1st");

        Widgets.EndSection();

        GUILayout.EndVertical();
    }
}
