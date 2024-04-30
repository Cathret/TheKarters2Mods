using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace TheKarters2Mods.Patches;

public class FastFallOnPress_ConfigData
{
    public ConfigEntry<float> ConfigFastFallRate { get; set; }
    public ConfigEntry<float> ConfigMinimumJoystickInputBeforeFastFall { get; set; }
    public ConfigEntry<float> ConfigMinimumJumpTimeBeforeFastFall { get; set; }
}

[HarmonyPatch(typeof(PixelKartPhysics), "FixedUpdate")]
public class FastFallOnPress
{
    private static FastFallOnPress Instance { get; set; }
    
    private static float ms_fastFallRate = 200f;
    private static float ms_minimumJoystickInputBeforeFastFall = 0.1f;
    private static float ms_minimumJumpTimeBeforeFastFall = 0.5f;

    public static void LoadConfig(FastFallOnPress_ConfigData _configData)
    {
        ms_fastFallRate = _configData.ConfigFastFallRate.Value;
        ms_minimumJoystickInputBeforeFastFall = _configData.ConfigMinimumJoystickInputBeforeFastFall.Value;
        ms_minimumJumpTimeBeforeFastFall = _configData.ConfigMinimumJumpTimeBeforeFastFall.Value;

        AerialFastFallPlugin.Log.LogInfo($"Loaded FastFallRate value [{ms_fastFallRate}]");
        AerialFastFallPlugin.Log.LogInfo($"Loaded MinimumJoystickInputBeforeFastFall value [{ms_minimumJoystickInputBeforeFastFall}]");
        AerialFastFallPlugin.Log.LogInfo($"Loaded MinimumJumpTimeBeforeFastFall value [{ms_minimumJumpTimeBeforeFastFall}]");
    }
    
    public static void Patch(Harmony _harmony)
    {
        _harmony.PatchAll(typeof(FastFallOnPress));
    }

    public static void Prepare(MethodBase original)
    {
        AerialFastFallPlugin.Log.LogMessage(original == null ? "self loading" : "patch loading");
    }

    private static bool IsAllowedToFastFall(PixelKartPhysics _kartPhysics)
    {
        float timeSinceStartedJump = Time.time - _kartPhysics.fPhysicsKartJumped_InUnityTime;
        return !_kartPhysics.bWasGrounded && !_kartPhysics.bIsDrifting && timeSinceStartedJump >= ms_minimumJumpTimeBeforeFastFall;
    }

    private static float GetFastFallInputValue()
    {
        // We prioritize the Keyboard input if needed
        if (Input.GetKey(KeyCode.DownArrow))
            return 1.0f;
        
        return -Input.GetAxis("Vertical");
    }
    
    public static void Postfix(PixelKartPhysics __instance)
    {
        if (!IsAllowedToFastFall(__instance))
            return;
        
        AerialFastFallPlugin.Log.LogDebug("Fast Falling ready to activate");

        float verticalRate = GetFastFallInputValue();
        if (verticalRate > ms_minimumJoystickInputBeforeFastFall)
        {
            AerialFastFallPlugin.Log.LogDebug($"Fast Falling activated (rate [{verticalRate}])");
            __instance.kartController.AddVelocity(Vector3.down * Time.fixedDeltaTime * ms_fastFallRate * verticalRate);
        }
    }
    
    public static void Cleanup(MethodBase original)
    {
        AerialFastFallPlugin.Log.LogMessage(original == null ? "finished self loading" : "finished patch loading");
    }
}
