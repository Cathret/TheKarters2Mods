using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using TheKarters2Mods.Patches;

namespace TheKarters2Mods;

[BepInPlugin(TwitchBasicInteractions_BepInExInfo.PLUGIN_GUID, TwitchBasicInteractions_BepInExInfo.PLUGIN_NAME, TwitchBasicInteractions_BepInExInfo.PLUGIN_VERSION)]
public class TwitchBasicInteractionsPlugin : BasePlugin
{
    private static TwitchBasicInteractionsPlugin Instance { get; set; }

    internal new static ManualLogSource Log;

    private static Patches.TwitchBasicInteractions ms_twitchInteractions = new TwitchBasicInteractions(); 
    
    public override void Load()
    {
        Instance = this;
        
        TwitchBasicInteractionsPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {TwitchBasicInteractions_BepInExInfo.PLUGIN_GUID} ({TwitchBasicInteractions_BepInExInfo.PLUGIN_NAME}) is loaded!");

        ms_twitchInteractions.Init();
    }

    public override bool Unload()
    {
        ms_twitchInteractions.Destroy();
        
        return base.Unload();
    }
}
