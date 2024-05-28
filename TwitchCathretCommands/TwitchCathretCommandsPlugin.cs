using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods;

[BepInPlugin(TwitchCathretCommands_BepInExInfo.PLUGIN_GUID, TwitchCathretCommands_BepInExInfo.PLUGIN_NAME, TwitchCathretCommands_BepInExInfo.PLUGIN_VERSION)]
[BepInDependency(DisableLeaderboards_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(TwitchBasicCommandsSDK_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(TheKartersModdingAssistant.MyPluginInfo.PLUGIN_NAME, ">=0.2.0")]
public class TwitchCathretCommandsPlugin : BasePlugin
{
    public static TwitchCathretCommandsPlugin Instance { get; private set; }

    internal new static ManualLogSource Log;

    private static Patches.TwitchCathretCommands ms_twitchCommandsSDK = new Patches.TwitchCathretCommands(); 
    
    public override void Load()
    {
        Instance = this;
        
        TwitchCathretCommandsPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {TwitchCathretCommands_BepInExInfo.PLUGIN_GUID} ({TwitchCathretCommands_BepInExInfo.PLUGIN_NAME}) is loaded!");

        ms_twitchCommandsSDK.Init();
    }

    public override bool Unload()
    {
        ms_twitchCommandsSDK.Destroy();
        
        return base.Unload();
    }
}
