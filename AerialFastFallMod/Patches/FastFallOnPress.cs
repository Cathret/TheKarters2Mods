using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP.Utils.Collections;
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

    private class FastFallState
    {
        public bool pressedTriangle = false;
        public bool hasDodge = false;
    }
    
    private static readonly Dictionary<Ant_Player.EAntPlayerNumber, FastFallState> ms_fastfallStatePerPlayer = new Dictionary<Ant_Player.EAntPlayerNumber, FastFallState>();
    
    private static Ant_Player.EAntPlayerNumber GetPlayerId(Ant_Player _antPlayer) => _antPlayer.eAntPlayerNr;
    private static FastFallState GetFallStateForPlayer(Ant_Player _antPlayer) => ms_fastfallStatePerPlayer[GetPlayerId(_antPlayer)];
    
    public static void Patch(Harmony _harmony)
    {
        foreach (Ant_Player.EAntPlayerNumber onePlayerNb in Enum.GetValues<Ant_Player.EAntPlayerNumber>())
        {
            ms_fastfallStatePerPlayer[onePlayerNb] = new FastFallState();
        }
        
        _harmony.PatchAll(typeof(Patch_PixelKartPhysics_FixedUpdate));
        _harmony.PatchAll(typeof(Patch_Ant_KartInput_ProcessRacingInput));
    }

    [HarmonyPatch(typeof(PixelKartPhysics), nameof(PixelKartPhysics.FixedUpdate))]
    private class Patch_PixelKartPhysics_FixedUpdate
    {
        public static void Prepare(MethodBase original)
        {
            AerialFastFallPlugin.Log.LogMessage(original == null ? "self Patch_PixelKartPhysics_FixedUpdate loading" : "patch Patch_PixelKartPhysics_FixedUpdate loading");
        }

        private static IEnumerator StayInvincibleAfterFastFall(HpBarController _hpBarController)
        {
            AerialFastFallPlugin.Log.LogDebug($"IMMUNITY START");
            _hpBarController.ActivateImmunityNow(true);
            yield return new WaitForSeconds(ms_dodgeDurationAfterPress);
            _hpBarController.ActivateImmunityNow(false);
            AerialFastFallPlugin.Log.LogDebug($"IMMUNITY STOP");
        }
    
        public static void Postfix(PixelKartPhysics __instance)
        {
            PixelEasyCharMoveKartController currentPlayerKartController = __instance.kartController;
            FastFallState currentPlayerFastFallState = GetFallStateForPlayer(currentPlayerKartController.parentPlayer);
            if (!currentPlayerFastFallState.pressedTriangle)
                return;
            
            AerialFastFallPlugin.Log.LogDebug($"Pressed Triangle == true");
            
            if (!IsAllowedToFastFall(__instance))
            {
                currentPlayerFastFallState.pressedTriangle = false; // if we can't fast fall anymore, reset input
                currentPlayerFastFallState.hasDodge = false;
                return;
            }
            
            AerialFastFallPlugin.Log.LogDebug($"Allowed to Fast Fall, fast falling");
            
            currentPlayerKartController.AddVelocity(Vector3.down * Time.fixedDeltaTime * ms_fastFallRate);

            if (ms_shouldDodgeOnPress)
            {
                if (!currentPlayerFastFallState.hasDodge)
                {
                    AerialFastFallPlugin.Log.LogDebug($"Has not dodge yet, executing dodge");

                    HpBarController currentPlayerHpController = currentPlayerKartController.parentPlayer.hpBarController;
                    currentPlayerHpController.StartCoroutine(
                        StayInvincibleAfterFastFall(currentPlayerHpController)
                            .WrapToIl2Cpp());

                    currentPlayerFastFallState.hasDodge = true;
                }
            }
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
            Player player = __instance.player;
            if (player == null)
                return;

            if (player.GetButtonDown(REWIRED_INPUT_PS_TRIANGLE))
            {
                AerialFastFallPlugin.Log.LogDebug($"Pressed Triangle");
                
                GetFallStateForPlayer(__instance.antPlayer).pressedTriangle = true; // will be set to false after being used
            }
        }
    
        public static void Cleanup(MethodBase original)
        {
            AerialFastFallPlugin.Log.LogMessage(original == null ? "finished self Patch_Ant_KartInput_ProcessRacingInput loading" : "finished patch Patch_Ant_KartInput_ProcessRacingInput loading");
        }
    }
}
