using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace TheKarters2Mods;

[BepInPlugin(AerialFastFall_BepInExInfo.PLUGIN_GUID, AerialFastFall_BepInExInfo.PLUGIN_NAME, AerialFastFall_BepInExInfo.PLUGIN_VERSION)]
[BepInDependency(PluginDependencies.DISABLE_LEADERBOARDS_GUID)]
[BepInDependency(PluginDependencies.AUTO_RELOAD_CONFIG_GUID)]
public class AerialFastFallPlugin : BasePlugin
{
    internal new static ManualLogSource Log;

    private readonly Harmony m_harmony = new Harmony(AerialFastFall_BepInExInfo.PLUGIN_GUID);
    
    private static ConfigEntry<bool> ConfigEnableMod { get; set; }

    public override void Load()
    {
        AerialFastFallPlugin.Log = base.Log;

        ConfigEnableMod = Config.Bind("_Plugin", "EnableMod", true, new ConfigDescription("NO RUNTIME REFRESH.\nShould enable Aerial Fast Fall mod. If enabled, will disable leaderboards."));
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {AerialFastFall_BepInExInfo.PLUGIN_GUID} ({AerialFastFall_BepInExInfo.PLUGIN_NAME}) is loaded!");

        if (ConfigEnableMod.Value)
        {
            DisableLeaderboardsPlugin.Enable();
            BetInExTesting.Patches.AerialFastFall.Patch(this, m_harmony);
        }
    }
    
    public override bool Unload()
    {
        m_harmony.UnpatchSelf();
        
        return base.Unload();
    }
}
