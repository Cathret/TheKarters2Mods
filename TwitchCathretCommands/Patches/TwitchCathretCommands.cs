using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches;

public class TwitchCathretCommands : AAutoReloadConfig
{
    internal static TwitchCathretCommands Instance { get; private set; }
    
    private DateTime m_lastInteractionTime = DateTime.Now;
    
    internal ConfigEntry<bool> ConfigActivateCommands { get; private set; }
    
    internal ConfigEntry<bool> ConfigKillCommand { get; private set; }
    internal ConfigEntry<bool> ConfigRemoveHealthCommand { get; private set; }
    internal ConfigEntry<bool> ConfigAddHealthCommand { get; private set; }
    
    internal ConfigEntry<bool> ConfigSetReserveCommand { get; private set; }
    internal ConfigEntry<bool> ConfigAddReserveCommand { get; private set; }
    internal ConfigEntry<bool> ConfigRemoveReserveCommand { get; private set; }

    private static readonly IEnumerable<Type> m_allCathretCommandsTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
        .Where(_p => typeof(ITwitchCommand).IsAssignableFrom(_p) && _p != typeof(ITwitchCommand));

    private readonly List<ITwitchCommand> m_allCathretCommandsInstances =
        m_allCathretCommandsTypes.Select(_type => (ITwitchCommand)Activator.CreateInstance(_type)).ToList();

    public void Init()
    {
        Instance = this;
        
        RegisterToAutoReload();
        LoadConfig();
    }

    public void Destroy()
    {
        TryUnregisterFromAutoReload();
    }

    private void EnableActivated()
    {
        foreach (ITwitchCommand oneCommand in m_allCathretCommandsInstances)
        {
            if (oneCommand.IsActivated())
            {
                TwitchBasicCommandsSDK.Instance.RegisterCommand(oneCommand);
            }
        }
        
        TwitchCathretCommandsPlugin.Log.LogDebug($"Twitch Cathret Commands Enabled");
    }

    private void DisableAll()
    {
        foreach (ITwitchCommand oneCommand in m_allCathretCommandsInstances)
        {
            TwitchBasicCommandsSDK.Instance.UnregisterCommand(oneCommand);
        }
        
        TwitchCathretCommandsPlugin.Log.LogDebug($"Twitch Cathret Commands Disabled");
    }

    private void RefreshCommands()
    {
        DisableAll();
        
        if (ConfigActivateCommands.Value)
            EnableActivated();
    }

    public override BasePlugin GetPlugin()
    {
        return TwitchCathretCommandsPlugin.Instance;
    }

    protected override void LoadConfig()
    {
        ConfigActivateCommands = TwitchCathretCommandsPlugin.Instance.Config.Bind("_Plugin", "Activate Mod", true, new ConfigDescription("Should the mod be activated and read commands from Twitch Chat"));
        
        ConfigKillCommand = TwitchCathretCommandsPlugin.Instance.Config.Bind("TwitchCathretCommands", "Activate Kill Command", true, new ConfigDescription("Usage: kill [karter name|pos X|humans|ais]"));
        ConfigAddHealthCommand = TwitchCathretCommandsPlugin.Instance.Config.Bind("TwitchCathretCommands", "Activate Add Health Command", true, new ConfigDescription("Usage: gain hp [X]"));
        ConfigRemoveHealthCommand = TwitchCathretCommandsPlugin.Instance.Config.Bind("TwitchCathretCommands", "Activate Remove Health Command", true, new ConfigDescription("Usage: lose hp [X]"));

        ConfigSetReserveCommand = TwitchCathretCommandsPlugin.Instance.Config.Bind("TwitchCathretCommands", "Activate Set Reserve Command", true, new ConfigDescription("Usage: reserve set X"));
        ConfigAddReserveCommand = TwitchCathretCommandsPlugin.Instance.Config.Bind("TwitchCathretCommands", "Activate Add Reserve Command", true, new ConfigDescription("Usage: reserve gain [X]"));
        ConfigRemoveReserveCommand = TwitchCathretCommandsPlugin.Instance.Config.Bind("TwitchCathretCommands", "Activate Remove Reserve Command", true, new ConfigDescription("Usage: reserve lose [X]"));

        RefreshCommands();
    }

    public override string GetPatchName()
    {
        return "Twitch Basic Interactions";
    }
}