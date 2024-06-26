﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods;

[BepInPlugin(TwitchBasicCommandsSDK_BepInExInfo.PLUGIN_GUID, TwitchBasicCommandsSDK_BepInExInfo.PLUGIN_NAME, TwitchBasicCommandsSDK_BepInExInfo.PLUGIN_VERSION)]
[BepInDependency(DisableLeaderboards_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(TwitchIntegrationSDK_BepInExInfo.PLUGIN_GUID)]
public class TwitchBasicCommandsSDKPlugin : BasePlugin
{
    public static TwitchBasicCommandsSDKPlugin Instance { get; private set; }

    internal new static ManualLogSource Log;

    private static Patches.TwitchBasicCommandsSDK ms_twitchCommandsSDK = new Patches.TwitchBasicCommandsSDK(); 
    
    public override void Load()
    {
        Instance = this;
        
        TwitchBasicCommandsSDKPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {TwitchBasicCommandsSDK_BepInExInfo.PLUGIN_GUID} ({TwitchBasicCommandsSDK_BepInExInfo.PLUGIN_NAME}) is loaded!");

        ms_twitchCommandsSDK.Init();
    }

    public override bool Unload()
    {
        ms_twitchCommandsSDK.Destroy();
        
        return base.Unload();
    }
}
