using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods.Patches;

public class TwitchBasicCommandsSDK : AAutoReloadConfig
{
    public static TwitchBasicCommandsSDK Instance { get; private set; }
    
    private DateTime m_lastInteractionTime = DateTime.Now;
    
    private ConfigEntry<bool> ConfigActivateInteractions { get; set; }
    private ConfigEntry<string> ConfigCommandPrefix { get; set; }
    private ConfigEntry<float> ConfigMinTimeBetweenInteractionsInSeconds { get; set; }

    private bool m_isActive = false;
    private string m_prefix = "#tk2";
    private float m_minTimeBetweenInteractions = 5.0f;

    private readonly List<ITwitchCommand> m_listBasicCommands = new List<ITwitchCommand>();
    
    public void Init()
    {
        if (Instance != null)
        {
            TwitchBasicCommandsSDKPlugin.Log.LogError($"Instance of Basic Commands SDK already existing, cancel Init");
            return;
        }
        
        Instance = this;
        
        RegisterToAutoReload();
        LoadConfig();
    }

    public void Destroy()
    {
        if (m_isActive)
            DisableMod();
        
        TryUnregisterFromAutoReload();

        Instance = null;
    }

    public void RegisterCommand(ITwitchCommand _twitchCommand)
    {
        if (!m_listBasicCommands.Contains(_twitchCommand))
        {
            m_listBasicCommands.Add(_twitchCommand);
        }
    }
    
    public void UnregisterCommand(ITwitchCommand _twitchCommand)
    {
        if (m_listBasicCommands.Contains(_twitchCommand))
        {
            m_listBasicCommands.Remove(_twitchCommand);
        }
    }

    private void EnableMod()
    {
        TwitchIntegrationSDK.OnTwitchChatMessage += OnTwitchChatMessage;

        m_isActive = true;
        
        TwitchBasicCommandsSDKPlugin.Log.LogDebug($"Twitch Basic Interaction Enabled");
    }

    private void DisableMod()
    {
        TwitchIntegrationSDK.OnTwitchChatMessage -= OnTwitchChatMessage;
        
        m_isActive = false;
        
        TwitchBasicCommandsSDKPlugin.Log.LogDebug($"Twitch Basic Interaction Disabled");
    }

    private bool CanInteract()
    {
        TimeSpan timeSinceLastInteraction = DateTime.Now - m_lastInteractionTime;
        return timeSinceLastInteraction.Seconds > m_minTimeBetweenInteractions;
    }

    private void ResetInteractionTimer()
    {
        m_lastInteractionTime = DateTime.Now;
    }

    private void OnTwitchChatMessage(string _user, string _message)
    {
        if (!CanInteract())
            return;
        
        string[] splittedMessage = _message.Split(' ');
        if (!splittedMessage[0].Equals(m_prefix))
            return;
        
        foreach (ITwitchCommand oneTwitchCommand in m_listBasicCommands)
        {
            if (oneTwitchCommand.ShouldExecuteCommand(_user, splittedMessage))
            {
                if (oneTwitchCommand.ExecuteCommand(_user, splittedMessage))
                {
                    TwitchIntegrationSDK.TwitchChatManager.WriteToChat(oneTwitchCommand.CommandFeedback(_user, splittedMessage));
                    ResetInteractionTimer();
                    return;
                }
            }
        }
    }

    public override BasePlugin GetPlugin()
    {
        return TwitchBasicCommandsSDKPlugin.Instance;
    }

    protected override void LoadConfig()
    {
        ConfigActivateInteractions = TwitchBasicCommandsSDKPlugin.Instance.Config.Bind("_Plugin", "Activate Mod", true, new ConfigDescription("Should the mod be activated and interact with Twitch Chat"));
        
        ConfigMinTimeBetweenInteractionsInSeconds = TwitchBasicCommandsSDKPlugin.Instance.Config.Bind("TwitchBasicCommandsSDK", "Minimum Time Between Interactions", 5.0f, new ConfigDescription("In Seconds.\nMinimum time between two interactions from Twitch Chat."));
        ConfigCommandPrefix = TwitchBasicCommandsSDKPlugin.Instance.Config.Bind("TwitchBasicCommandsSDK", "Command Prefix", "#tk2", new ConfigDescription("Prefix of the commands to be used by this Twitch Interaction Bot."));
        
        m_minTimeBetweenInteractions = ConfigMinTimeBetweenInteractionsInSeconds.Value;
        m_prefix = ConfigCommandPrefix.Value;

        TwitchBasicCommandsSDKPlugin.Log.LogInfo($"Loaded MinTimeBetweenInteractionsInSeconds value [{m_minTimeBetweenInteractions}]");
        TwitchBasicCommandsSDKPlugin.Log.LogInfo($"Loaded CommandPrefix value [{m_prefix}]");

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
        return "Twitch Basic Commands SDK";
    }
}