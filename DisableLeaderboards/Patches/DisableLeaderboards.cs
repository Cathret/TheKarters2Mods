using System.Reflection;
using HarmonyLib;
using TheKarters2Mods;

namespace BetInExTesting.Patches;

[HarmonyPatch(typeof(KartersLeaderboardsManager), "RaceFinished_UploadLeaderboard")]
public class DisableLeaderboards
{
    public static void Prepare(MethodBase original)
    {
        if (original == null)
            DisableLeaderboardsPlugin.Log.LogMessage("Disabling Leaderboards");
    }
    
    public static void Cleanup(MethodBase original)
    {
        if (original == null)
            DisableLeaderboardsPlugin.Log.LogMessage("Leaderboards Disabled");
    }
    
    public static bool Prefix()
    {
        return false;
    }
}