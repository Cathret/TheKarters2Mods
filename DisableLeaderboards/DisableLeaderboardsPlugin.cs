using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace TheKarters2Mods;

[BepInPlugin(DisableLeaderboards_BepInExInfo.PLUGIN_GUID, DisableLeaderboards_BepInExInfo.PLUGIN_NAME, DisableLeaderboards_BepInExInfo.PLUGIN_VERSION)]
public class DisableLeaderboardsPlugin : BasePlugin
{
    internal new static ManualLogSource Log;

    private readonly Harmony m_harmony = new Harmony(DisableLeaderboards_BepInExInfo.PLUGIN_GUID);

    public override void Load()
    {
        DisableLeaderboardsPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {DisableLeaderboards_BepInExInfo.PLUGIN_GUID} ({DisableLeaderboards_BepInExInfo.PLUGIN_NAME}) is loaded!");
        
        m_harmony.PatchAll(typeof(BetInExTesting.Patches.DisableLeaderboards));
    }

    public override bool Unload()
    {
        m_harmony.UnpatchSelf();
        
        return base.Unload();
    }
}
