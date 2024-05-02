using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using Rewired;
using UnityEngine;

namespace TheKarters2Mods.Patches;

public class FastFallOnPress_ConfigData
{
    public ConfigEntry<bool> ConfigShouldDodgeOnPress { get; set; }
    public ConfigEntry<float> ConfigDodgeDurationAfterPress { get; set; }
    public ConfigEntry<float> ConfigFastFallRate { get; set; }
    public ConfigEntry<float> ConfigMinimumJumpTimeBeforeFastFall { get; set; }
}

public class FastFallOnPress : AFastFall
{
    private static FastFallOnPress Instance { get; set; }
    
    private static bool ms_shouldDodgeOnPress = true;
    private static float ms_dodgeDurationAfterPress = 0.5f;

    public static void LoadConfig(FastFallOnPress_ConfigData _configData)
    {
        ms_shouldDodgeOnPress = _configData.ConfigShouldDodgeOnPress.Value;
        ms_dodgeDurationAfterPress = _configData.ConfigDodgeDurationAfterPress.Value;
        ms_fastFallRate = _configData.ConfigFastFallRate.Value;
        ms_minimumJumpTimeBeforeFastFall = _configData.ConfigMinimumJumpTimeBeforeFastFall.Value;

        AerialFastFallPlugin.Log.LogInfo($"Loaded ShouldDodgeOnPress value [{ms_shouldDodgeOnPress}]");
        AerialFastFallPlugin.Log.LogInfo($"Loaded DodgeDurationAfterPress value [{ms_dodgeDurationAfterPress}]");
        AerialFastFallPlugin.Log.LogInfo($"Loaded FastFallRate value [{ms_fastFallRate}]");
        AerialFastFallPlugin.Log.LogInfo($"Loaded MinimumJumpTimeBeforeFastFall value [{ms_minimumJumpTimeBeforeFastFall}]");
    }
    
    public static void Patch(Harmony _harmony)
    {
        _harmony.PatchAll(typeof(Patch_PixelKartPhysics_FixedUpdate));
        _harmony.PatchAll(typeof(Patch_Ant_KartInput_ProcessRacingInput));
    }

    private static bool ms_pressedTriangle = false;

    [HarmonyPatch(typeof(PixelKartPhysics), nameof(PixelKartPhysics.FixedUpdate))]
    private class Patch_PixelKartPhysics_FixedUpdate
    {
        public static void Prepare(MethodBase original)
        {
            AerialFastFallPlugin.Log.LogMessage(original == null ? "self Patch_PixelKartPhysics_FixedUpdate loading" : "patch Patch_PixelKartPhysics_FixedUpdate loading");
        }
    
        public static void Postfix(PixelKartPhysics __instance)
        {
            if (!ms_pressedTriangle)
                return;
            
            if (!IsAllowedToFastFall(__instance))
            {
                ms_pressedTriangle = false; // if we can't fast fall anymore, reset input
                return;
            }

            AerialFastFallPlugin.Log.LogDebug($"Fast Falling activated");
            
            __instance.kartController.AddVelocity(Vector3.down * Time.fixedDeltaTime * ms_fastFallRate);
        }
    
        public static void Cleanup(MethodBase original)
        {
            AerialFastFallPlugin.Log.LogMessage(original == null ? "finished self Patch_PixelKartPhysics_FixedUpdate loading" : "finished patch Patch_PixelKartPhysics_FixedUpdate loading");
        }
    }
    
    [HarmonyPatch(typeof(Ant_KartInput), nameof(Ant_KartInput.ProcessRacingInput))]
    private class Patch_Ant_KartInput_ProcessRacingInput
    {
        private const string REWIRED_INPUT_PS_TRIANGLE = "MenuTriangle";
        
        public static void Prepare(MethodBase original)
        {
            AerialFastFallPlugin.Log.LogMessage(original == null ? "self Patch_Ant_KartInput_ProcessRacingInput loading" : "patch Patch_Ant_KartInput_ProcessRacingInput loading");
        }
        
        public static void Postfix(Ant_KartInput __instance)
        {
            AerialFastFallPlugin.Log.LogDebug($"ProcessRacingInput - postfix");

            Player player = __instance.player;
            if (player == null)
                return;

            AerialFastFallPlugin.Log.LogDebug($"Rewired is available");
            
            if (player.GetButtonDown(REWIRED_INPUT_PS_TRIANGLE))
            {
                AerialFastFallPlugin.Log.LogDebug($"Pressed Triangle");
                ms_pressedTriangle = true; // set to false after being used
            }
        }
    
        public static void Cleanup(MethodBase original)
        {
            AerialFastFallPlugin.Log.LogMessage(original == null ? "finished self Patch_Ant_KartInput_ProcessRacingInput loading" : "finished patch Patch_Ant_KartInput_ProcessRacingInput loading");
        }
    }
}
