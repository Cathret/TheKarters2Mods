using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods;

[BepInPlugin(TwitchIntegrationSDK_BepInExInfo.PLUGIN_GUID, TwitchIntegrationSDK_BepInExInfo.PLUGIN_NAME, TwitchIntegrationSDK_BepInExInfo.PLUGIN_VERSION)]
[BepInDependency(DisableLeaderboards_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID)]
public class TwitchIntegrationSDKPlugin : BasePlugin
{
    private static TwitchIntegrationSDKPlugin Instance { get; set; }

    internal new static ManualLogSource Log;

    private static ConfigEntry<bool> ConfigEnableMod { get; set; }

    private Patches.TwitchIntegrationSDK_Patcher m_twitchIntegrationSDKPatcher = new Patches.TwitchIntegrationSDK_Patcher();

    public override void Load()
    {
        // Avoid our DontDestroyOnLoad GameObject to be destroyed too early
        InitThreading();
    }
    
    public override bool Unload()
    {
        m_twitchIntegrationSDKPatcher.Unload();
        
        return base.Unload();
    }
    
    private void Init()
    {
        Instance = this;
        
        TwitchIntegrationSDKPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {TwitchIntegrationSDK_BepInExInfo.PLUGIN_GUID} ({TwitchIntegrationSDK_BepInExInfo.PLUGIN_NAME}) is loaded!");

        m_twitchIntegrationSDKPatcher.Load(this);
    }
    
    private const int TIME_BEFORE_LOAD_MS = 5000;
    
    private void InitThreading()
    {
        new Thread(() =>
        {
            Thread.Sleep(TIME_BEFORE_LOAD_MS);

            Init();

        }).Start();
    }
}
