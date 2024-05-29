using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using TheKarters2Mods.Patches;

namespace TheKarters2Mods;

[BepInPlugin(AerialFastFall_BepInExInfo.PLUGIN_GUID, AerialFastFall_BepInExInfo.PLUGIN_NAME, AerialFastFall_BepInExInfo.PLUGIN_VERSION)]
[BepInDependency(DisableLeaderboards_BepInExInfo.PLUGIN_GUID)]
[BepInDependency(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID)]
public class AerialFastFallPlugin : BasePlugin
{
    public static AerialFastFallPlugin Instance { get; private set; }
    
    internal new static ManualLogSource Log;
    
    private static ConfigEntry<bool> ConfigEnableMod { get; set; }

    private Patches.AerialFastFall_Patcher m_aerialFastFall = new AerialFastFall_Patcher();

    public override void Load()
    {
        Instance = this;
        
        AerialFastFallPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {AerialFastFall_BepInExInfo.PLUGIN_GUID} ({AerialFastFall_BepInExInfo.PLUGIN_NAME}) is loaded!");

        m_aerialFastFall.Load(this);
    }
    
    public override bool Unload()
    {
        m_aerialFastFall.Unload();
        
        return base.Unload();
    }
}
