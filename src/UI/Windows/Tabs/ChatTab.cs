using UnityEngine;

namespace PanaM;

public class ChatTab : ITab
{
    public string name => "Chat";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        Widgets.BeginSection("General");

        CheatToggles.enableChat = Widgets.Toggle(CheatToggles.enableChat, "Enable Chat");

        CheatToggles.bypassUrlBlock = Widgets.Toggle(CheatToggles.bypassUrlBlock, "Bypass URL Block");

        CheatToggles.lowerRateLimits = Widgets.Toggle(CheatToggles.lowerRateLimits, "Lower Rate Limits");

        Widgets.EndSection();

        GUILayout.Space(4);

        Widgets.BeginSection("Textbox");

        CheatToggles.unlockCharacters = Widgets.Toggle(CheatToggles.unlockCharacters, "Unlock Extra Characters");

        CheatToggles.longerMessages = Widgets.Toggle(CheatToggles.longerMessages, "Allow Longer Messages");

        CheatToggles.unlockClipboard = Widgets.Toggle(CheatToggles.unlockClipboard, "Unlock Clipboard");

        Widgets.EndSection();

        GUILayout.EndVertical();
    }
}
