using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TheKartersModdingAssistant;
using TheKartersModdingAssistant.Event;

namespace TheKarters2Mods.Patches;

public class TwitchBasicInteractions
{
    public void Init()
    {
        TwitchIntegrationSDK.OnTwitchChatMessage += OnTwitchChatMessage;
    }

    public void Destroy()
    {
        TwitchIntegrationSDK.OnTwitchChatMessage -= OnTwitchChatMessage;
    }

    private void OnTwitchChatMessage(string _user, string _message)
    {
        if (_user.Equals("cathret") && _message.Equals("death"))
        {
            List<Player> allPlayers = Player.GetPlayers();
            foreach (Player onePlayer in allPlayers)
            {
                if (onePlayer.IsHuman())
                {
                    onePlayer.SetCurrentHealth(0);
                }
            }
        }
        
        if (_user.Equals("cathret") && _message.Equals("boost"))
        {
            List<Player> allPlayers = Player.GetPlayers();
            foreach (Player onePlayer in allPlayers)
            {
                if (onePlayer.IsHuman())
                {
                    onePlayer.SetCurrentBoostNumber(200);
                }
            }
        }
    }
}