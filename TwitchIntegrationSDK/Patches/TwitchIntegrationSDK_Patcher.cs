using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;

namespace TheKarters2Mods.Patches;

public class TwitchIntegrationSDK_Patcher : AAutoReloadConfig
{
    private TwitchIntegrationSDKPlugin m_plugin = null;
    
    private bool m_isEnabled = false;
    
    private ConfigEntry<bool> ConfigEnableTwitchIntegration { get; set; }

    private readonly TwitchIntegrationSDK_ConfigData m_configTwitchIntegrationSDK = new TwitchIntegrationSDK_ConfigData();

    public bool Load(TwitchIntegrationSDKPlugin _plugin)
    {
        if (_plugin == null)
        {
            TwitchIntegrationSDKPlugin.Log.LogError("Plugin not found while patching, AerialFastFall will not be loaded");
            return false;
        }
        
        m_plugin = _plugin;
        
        //RegisterToAutoReload();
        InitialPatch();

        return true;
    }

    public void Unload()
    {
        if (m_isEnabled)
            DisablePatch();
    }

    private void BindConfigs()
    {
        ConfigEnableTwitchIntegration = m_plugin.Config.Bind("_Plugin", "EnableTwitchIntegration", true, new ConfigDescription("Should enable Twitch Integration mod. If enabled, will disable leaderboards."));

        m_configTwitchIntegrationSDK.ConfigUserName = m_plugin.Config.Bind("TwitchIntegration", "Username", "__REPLACE_ME__", new ConfigDescription("Username of the Bot in Twitch Chat."));
        m_configTwitchIntegrationSDK.ConfigOAuthToken = m_plugin.Config.Bind("TwitchIntegration", "OAuth Token", "__REPLACE_ME__", new ConfigDescription("/!!\\ DO NOT SHARE THIS INFORMATION /!!\\\nOAuth Token of the Authorization given on your Twitch Account."));
        m_configTwitchIntegrationSDK.ConfigChannelName = m_plugin.Config.Bind("TwitchIntegration", "Channel Name", "__REPLACE_ME__", new ConfigDescription("Channel name of the Twitch Channel to connect to."));
    }

    private void RefreshPatch()
    {
        if (m_isEnabled)
        {
            if (!ConfigEnableTwitchIntegration.Value)
                DisablePatch();
        }
        else
        {
            if (ConfigEnableTwitchIntegration.Value)
                EnablePatch();
        }
    }

    private void EnablePatch()
    {
        TwitchIntegrationSDKPlugin.Log.LogMessage("Enabling Twitch Integration");
            
        TwitchIntegrationSDK.Patch();
        
        m_isEnabled = true;
    }

    private void DisablePatch()
    {
        TwitchIntegrationSDKPlugin.Log.LogMessage("Disabling Twitch Integration");

        TwitchIntegrationSDK.UnPatch();

        m_isEnabled = false;
    }
    
    public override BasePlugin GetPlugin()
    {
        return m_plugin;
    }

    private void InitialPatch() => LoadConfig();
    protected override void LoadConfig()
    {
        BindConfigs();

        if (ConfigEnableTwitchIntegration.Value)
        {
            TwitchIntegrationSDK.LoadConfig(m_configTwitchIntegrationSDK);
        }
        
        RefreshPatch();
    }
    
    public override string GetPatchName()
    {
        return "Twitch Integration SDK Patch";
    }
}