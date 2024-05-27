using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TheKartersModdingAssistant;
using TheKartersModdingAssistant.Event;

namespace TheKarters2Mods.Patches;

public class TwitchBasicInteractions
{
    private string m_prefix = "#tk2";
    
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
        string[] splittedMessage = _message.Split(' ');

        if (!splittedMessage[0].Equals(m_prefix))
            return;
        
        if (splittedMessage.Length == 2 && splittedMessage[1].Equals("death"))
        {
            TwitchBasicInteractionsPlugin.Log.LogDebug($"{_user} requires death");

            TwitchIntegrationSDK.TwitchChatManager.WriteToChat("Sad day for life today...");

            List<Player> allPlayers = Player.GetPlayers();
            foreach (Player onePlayer in allPlayers)
            {
                if (onePlayer.IsHuman())
                {
                    TwitchBasicInteractionsPlugin.Log.LogDebug($"Killing Player");

                    onePlayer.uHpBarController.Death();
                }
            }
        }

        if (splittedMessage.Length >= 2 && splittedMessage[1].StartsWith("boost"))
        {
            TwitchBasicInteractionsPlugin.Log.LogDebug($"{_user} requires boost");

            int boostValue = 85;
            if (splittedMessage.Length == 3)
            {
                boostValue = int.Parse(splittedMessage[2]);
            }

            List<Player> allPlayers = Player.GetPlayers();
            foreach (Player onePlayer in allPlayers)
            {
                if (onePlayer.IsHuman())
                {
                    TwitchBasicInteractionsPlugin.Log.LogDebug($"Setting player raw reserve to {boostValue}");

                    onePlayer.SetCurrentReserve(boostValue);
                }
            }
        }
    }
}