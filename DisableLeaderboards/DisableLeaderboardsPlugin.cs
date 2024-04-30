using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace TheKarters2Mods;

[BepInPlugin(DisableLeaderboards_BepInExInfo.PLUGIN_GUID, DisableLeaderboards_BepInExInfo.PLUGIN_NAME, DisableLeaderboards_BepInExInfo.PLUGIN_VERSION)]
public class DisableLeaderboardsPlugin : BasePlugin
{
    private static DisableLeaderboardsPlugin Instance { get; set; }

    internal new static ManualLogSource Log;

    private readonly Harmony m_harmony = new Harmony(DisableLeaderboards_BepInExInfo.PLUGIN_GUID);

    private static bool ms_shouldDisableLeaderboards = false;
    public static bool ShouldDisableLeaderboards { get { return ms_shouldDisableLeaderboards; } }

    public static void Enable()
    {
        ms_shouldDisableLeaderboards = true;

        Instance?.PatchDisableLeaderboards();
    }

    private void PatchDisableLeaderboards()
    { 
        m_harmony.PatchAll(typeof(BetInExTesting.Patches.DisableLeaderboards));
    }

    public override void Load()
    {
        Instance = this;
        
        DisableLeaderboardsPlugin.Log = base.Log;
        
        // Plugin startup logic
        Log.LogMessage($"Plugin {DisableLeaderboards_BepInExInfo.PLUGIN_GUID} ({DisableLeaderboards_BepInExInfo.PLUGIN_NAME}) is loaded!");

        if (ms_shouldDisableLeaderboards)
            PatchDisableLeaderboards();
    }

    public override bool Unload()
    {
        if (ms_shouldDisableLeaderboards)
           m_harmony.UnpatchSelf();
        
        return base.Unload();
    }
}
