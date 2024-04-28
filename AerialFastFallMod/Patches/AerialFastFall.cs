using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using TheKarters2Mods;
using UnityEngine;

namespace BetInExTesting.Patches;

[HarmonyPatch(typeof(PixelKartPhysics), "FixedUpdate")]
public class AerialFastFall : AAutoReloadConfig
{
    private static AerialFastFall Instance { get; set; }
    
    private static AerialFastFallPlugin ms_plugin = null;
    
    private static ConfigEntry<float> ConfigFastFallRate { get; set; }
    private static ConfigEntry<float> ConfigMinimumJoystickInputBeforeFastFall { get; set; }
    private static ConfigEntry<float> ConfigMinimumJumpTimeBeforeFastFall { get; set; }
    
    private static float ms_fastFallRate = 200f;
    private static float ms_minimumJoystickInputBeforeFastFall = 0.1f;
    private static float ms_minimumJumpTimeBeforeFastFall = 0.5f;

    private static void LogDebug(string _text) => AerialFastFallPlugin.Log.LogDebug($"[AerialFastFall]: {_text}");
    private static void LogInfo(string _text) => AerialFastFallPlugin.Log.LogInfo($"[AerialFastFall]: {_text}");
    private static void LogMessage(string _text) => AerialFastFallPlugin.Log.LogMessage($"[AerialFastFall]: {_text}");
    private static void LogError(string _text) => AerialFastFallPlugin.Log.LogError($"[AerialFastFall]: {_text}");

    public static void Patch(AerialFastFallPlugin _plugin, Harmony _harmony)
    {
        if (_plugin == null)
        {
            LogError("Plugin not found while patching, AerialFastFall will not be loaded");
            return;
        }
        
        ms_plugin = _plugin;
        
        Instance = new AerialFastFall();
        Instance.RegisterToAutoReload();
        Instance.LoadConfig();
        
        _harmony.PatchAll(typeof(AerialFastFall));
    }

    private static void BindConfigs()
    {
        ConfigFastFallRate = ms_plugin.Config.Bind("AerialFastFall", "FastFallRate", 200f, new ConfigDescription("Fast Fall Rate. The higher, the faster the kart will be directed to the ground.", new AcceptableValueRange<float>(0f, 1000f)));
        ConfigMinimumJoystickInputBeforeFastFall = ms_plugin.Config.Bind("AerialFastFall", "MinimumJoystickInputBeforeFastFall", 0.1f, new ConfigDescription("Minimum Joystick Input Before Fast Fall in Seconds. Fast Fall Deadzone. While the joystick input is lower than this value, the Fast Fall will not be triggered.", new AcceptableValueRange<float>(0f, 1f)));
        ConfigMinimumJumpTimeBeforeFastFall = ms_plugin.Config.Bind("AerialFastFall", "MinimumJumpTimeBeforeFastFall", 0.5f, new ConfigDescription("Minimum Jump Time Before Fast Fall in Seconds. Minimum time after being airborne before Fast Fall is allowed.", new AcceptableValueRange<float>(0f, 5f)));
        
        ms_fastFallRate = ConfigFastFallRate.Value;
        ms_minimumJoystickInputBeforeFastFall = ConfigMinimumJoystickInputBeforeFastFall.Value;
        ms_minimumJumpTimeBeforeFastFall = ConfigMinimumJumpTimeBeforeFastFall.Value;

        LogInfo($"Loaded FastFallRate value [{ms_fastFallRate}]");
        LogInfo($"Loaded MinimumJoystickInputBeforeFastFall value [{ms_minimumJoystickInputBeforeFastFall}]");
        LogInfo($"Loaded MinimumJumpTimeBeforeFastFall value [{ms_minimumJumpTimeBeforeFastFall}]");
    }

    public static void Prepare(MethodBase original)
    {
        LogMessage(original == null ? "self loading" : "patch loading");
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
        
        LogDebug("Fast Falling ready to activate");

        float verticalRate = GetFastFallInputValue();
        if (verticalRate > ms_minimumJoystickInputBeforeFastFall)
        {
            LogDebug($"Fast Falling activated (rate [{verticalRate}])");
            __instance.kartController.AddVelocity(Vector3.down * Time.fixedDeltaTime * ms_fastFallRate * verticalRate);
        }
    }
    
    public static void Cleanup(MethodBase original)
    {
        LogMessage(original == null ? "finished self loading" : "finished patch loading");
    }

    public override BasePlugin GetPlugin()
    {
        return ms_plugin;
    }

    protected override void LoadConfig()
    {
        BindConfigs();
    }
}
