using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches;

public class TwitchBasicInteractions : AAutoReloadConfig
{
    private DateTime m_lastInteractionTime = DateTime.Now;
    
    private ConfigEntry<bool> ConfigActivateInteractions { get; set; }
    private ConfigEntry<string> ConfigCommandPrefix { get; set; }
    private ConfigEntry<float> ConfigMinTimeBetweenInteractionsInSeconds { get; set; }

    private bool m_isActive = false;
    private string m_prefix = "#tk2";
    private float m_minTimeBetweenInteractions = 5.0f;
    
    public void Init()
    {
        RegisterToAutoReload();
        LoadConfig();
    }

    public void Destroy()
    {
        if (m_isActive)
            DisableMod();
        
        TryUnregisterFromAutoReload();
    }

    private void EnableMod()
    {
        TwitchIntegrationSDK.OnTwitchChatMessage += OnTwitchChatMessage;

        m_isActive = true;
        
        TwitchBasicInteractionsPlugin.Log.LogDebug($"Twitch Basic Interaction Enabled");
    }

    private void DisableMod()
    {
        TwitchIntegrationSDK.OnTwitchChatMessage -= OnTwitchChatMessage;
        
        m_isActive = false;
        
        TwitchBasicInteractionsPlugin.Log.LogDebug($"Twitch Basic Interaction Disabled");
    }

    private void OnTwitchChatMessage(string _user, string _message)
    {
        TimeSpan timeSinceLastInteraction = DateTime.Now - m_lastInteractionTime;
        if (timeSinceLastInteraction.Seconds <= m_minTimeBetweenInteractions)
        {
            return;
        }
        
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
            
            m_lastInteractionTime = DateTime.Now;
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
            
            m_lastInteractionTime = DateTime.Now;
        }
    }

    public override BasePlugin GetPlugin()
    {
        return TwitchBasicInteractionsPlugin.Instance;
    }

    protected override void LoadConfig()
    {
        ConfigActivateInteractions = TwitchBasicInteractionsPlugin.Instance.Config.Bind("_Plugin", "Activate Mod", true, new ConfigDescription("Should the mod be activated and interact with Twitch Chat"));
        
        ConfigMinTimeBetweenInteractionsInSeconds = TwitchBasicInteractionsPlugin.Instance.Config.Bind("TwitchBasicInteractions", "Minimum Time Between Interactions", 5.0f, new ConfigDescription("In Seconds.\nMinimum time between two interactions from Twitch Chat."));
        ConfigCommandPrefix = TwitchBasicInteractionsPlugin.Instance.Config.Bind("TwitchBasicInteractions", "Command Prefix", "#tk2", new ConfigDescription("Prefix of the commands to be used by this Twitch Interaction Bot."));
        
        m_minTimeBetweenInteractions = ConfigMinTimeBetweenInteractionsInSeconds.Value;
        m_prefix = ConfigCommandPrefix.Value;

        TwitchBasicInteractionsPlugin.Log.LogInfo($"Loaded MinTimeBetweenInteractionsInSeconds value [{m_minTimeBetweenInteractions}]");
        TwitchBasicInteractionsPlugin.Log.LogInfo($"Loaded CommandPrefix value [{m_prefix}]");

        if (m_isActive != ConfigActivateInteractions.Value)
        {
            if (m_isActive)
                DisableMod();
            else
                EnableMod();
        }
    }

    public override string GetPatchName()
    {
        return "Twitch Basic Interactions";
    }
}