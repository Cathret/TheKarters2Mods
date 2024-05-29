using System;
using BepInEx.Configuration;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace TheKarters2Mods.Patches;

public class TwitchIntegrationSDK_ConfigData
{
    public ConfigEntry<bool> ConfigEnableDebug { get; set; }
    public ConfigEntry<string> ConfigUserName { get; set; }
    public ConfigEntry<string> ConfigOAuthToken { get; set; }
    public ConfigEntry<string> ConfigChannelName { get; set; }
}

public class TwitchIntegrationSDK
{
    public static TwitchIntegrationSDK Instance { get; private set; }
    
    private static GameObject TwitchObject { get; set; }
    public static Injections.TwitchIntegrationSDK_ChatManager TwitchChatManager { get; private set; }
    public static Injections.TwitchIntegrationSDK_EventManager TwitchEventManager { get; private set; }
    
    public static bool DebugEnabled { get; private set; }
    public static string TwitchUserName { get; private set; }
    public static string TwitchOAuthToken { get; private set; }
    public static string TwitchChannelName { get; private set; }

    public static Action<string, string> OnTwitchChatMessage
    {
        get => Injections.TwitchIntegrationSDK_EventManager.onTwitchChatMessage;
        set => Injections.TwitchIntegrationSDK_EventManager.onTwitchChatMessage = value;
    }

    public static void LoadConfig(TwitchIntegrationSDK_ConfigData _configData)
    {
        DebugEnabled = _configData.ConfigEnableDebug.Value;
        TwitchUserName = _configData.ConfigUserName.Value;
        TwitchOAuthToken = _configData.ConfigOAuthToken.Value;
        TwitchChannelName = _configData.ConfigChannelName.Value;

        TwitchIntegrationSDKPlugin.Log.LogDebug($"Debug Enabled? [{DebugEnabled}]");
        TwitchIntegrationSDKPlugin.Log.LogInfo($"Loaded UserName value [{TwitchUserName}]");
        TwitchIntegrationSDKPlugin.Log.LogInfo($"Loaded OAuth Token value [__HIDDEN__]");
        TwitchIntegrationSDKPlugin.Log.LogDebug($"Loaded OAuth Token value [{TwitchOAuthToken}]");
        TwitchIntegrationSDKPlugin.Log.LogInfo($"Loaded Channel Name value [{TwitchChannelName}]");
    }
    
    public static void Patch()
    {
        RegisterTwitchIntegrationTypesInIl2Cpp();
        CreateTwitchIntegrationObject();
    }

    public static void UnPatch()
    {
        DeleteInstanceGameObject();
    }

    private static void RegisterTwitchIntegrationTypesInIl2Cpp()
    {
        ClassInjector.RegisterTypeInIl2Cpp<Injections.TwitchIntegrationSDK_ChatManager>();
        ClassInjector.RegisterTypeInIl2Cpp<Injections.TwitchIntegrationSDK_EventManager>();
    }

    private static void CreateTwitchIntegrationObject()
    {
        TwitchIntegrationSDKPlugin.Log.LogDebug($"CREATING Twitch Integration GameObject");

        TwitchObject = new GameObject("TwitchIntegrationSDK_GO");
        UnityEngine.Object.DontDestroyOnLoad(TwitchObject);
        
        TwitchChatManager = TwitchObject.AddComponent<Injections.TwitchIntegrationSDK_ChatManager>();
        TwitchEventManager = TwitchObject.AddComponent<Injections.TwitchIntegrationSDK_EventManager>();
    }
    
    private static bool DeleteInstanceGameObject()
    {
        TwitchIntegrationSDKPlugin.Log.LogDebug($"DELETING Twitch Integration GameObject");

        if (TwitchObject == null)
            return false;
        
        UnityEngine.Object.Destroy(TwitchObject);

        TwitchChatManager = null;
        TwitchEventManager = null;
        
        return true;
    }
}
