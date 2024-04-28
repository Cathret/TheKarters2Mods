using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace TheKarters2Mods;

[BepInPlugin(AerialFastFall_BepInExInfo.PLUGIN_GUID, AerialFastFall_BepInExInfo.PLUGIN_NAME, AerialFastFall_BepInExInfo.PLUGIN_VERSION)]
[BepInDependency(DisableLeaderboardsDependency.PLUGIN_GUID)]
public class AerialFastFallPlugin : BasePlugin
{
    internal new static ManualLogSource Log;

    private readonly Harmony m_harmony = new Harmony(AerialFastFall_BepInExInfo.PLUGIN_GUID);

    public override void Load()
    {
        AerialFastFallPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {AerialFastFall_BepInExInfo.PLUGIN_GUID} ({AerialFastFall_BepInExInfo.PLUGIN_NAME}) is loaded!");
        
        Log.LogDebug("Debug Log working");
        Log.LogInfo("Info Log working");
        Log.LogMessage("Message Log working");
        Log.LogWarning("Warning Log working");
        Log.LogError("Error Log working");
        Log.LogFatal("Fatal Log working");

        BetInExTesting.Patches.AerialFastFall.Patch(this, m_harmony);
    }

    public override bool Unload()
    {
        m_harmony.UnpatchSelf();
        
        return base.Unload();
    }
}
