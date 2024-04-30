using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace TheKarters2Mods.Patches;

public class AerialFastFall_Patcher : AAutoReloadConfig
{
    private AerialFastFallPlugin m_plugin = null;
    
    private const string PATCH_GUID = AerialFastFall_BepInExInfo.PLUGIN_GUID + "AerialFastFallMod";
    private readonly Harmony m_harmony = new Harmony(PATCH_GUID);
    private bool m_isEnabled = false;

    private ConfigEntry<bool> ConfigEnableAerialFastFall { get; set; }
    
    private readonly FastFallOnPress_ConfigData m_configFastFallOnPress = new FastFallOnPress_ConfigData();

    public bool Load(AerialFastFallPlugin _plugin)
    {
        if (_plugin == null)
        {
            AerialFastFallPlugin.Log.LogError("Plugin not found while patching, AerialFastFall will not be loaded");
            return false;
        }
        
        m_plugin = _plugin;
        
        RegisterToAutoReload();
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
        ConfigEnableAerialFastFall = m_plugin.Config.Bind("_Plugin", "EnableAerialFastFall", true, new ConfigDescription("Should enable Aerial Fast Fall mod. If enabled, will disable leaderboards."));

        m_configFastFallOnPress.ConfigFastFallRate = m_plugin.Config.Bind("AerialFastFall", "FastFallRate", 200f, new ConfigDescription("Fast Fall Rate. The higher, the faster the kart will be directed to the ground.", new AcceptableValueRange<float>(0f, 1000f)));
        m_configFastFallOnPress.ConfigMinimumJoystickInputBeforeFastFall = m_plugin.Config.Bind("AerialFastFall", "MinimumJoystickInputBeforeFastFall", 0.1f, new ConfigDescription("Minimum Joystick Input Before Fast Fall in Seconds. Fast Fall Deadzone. While the joystick input is lower than this value, the Fast Fall will not be triggered.", new AcceptableValueRange<float>(0f, 1f)));
        m_configFastFallOnPress.ConfigMinimumJumpTimeBeforeFastFall = m_plugin.Config.Bind("AerialFastFall", "MinimumJumpTimeBeforeFastFall", 0.5f, new ConfigDescription("Minimum Jump Time Before Fast Fall in Seconds. Minimum time after being airborne before Fast Fall is allowed.", new AcceptableValueRange<float>(0f, 5f)));
    }

    private void RefreshPatch()
    {
        if (m_isEnabled)
        {
            if (!ConfigEnableAerialFastFall.Value)
                DisablePatch();
        }
        else
        {
            if (ConfigEnableAerialFastFall.Value)
                EnablePatch();
        }
    }

    private void EnablePatch()
    {
        AerialFastFallPlugin.Log.LogMessage("Enabling AerialFastFall");

        m_harmony.PatchAll(typeof(FastFallOnPress));
        m_isEnabled = true;
    }

    private void DisablePatch()
    {
        AerialFastFallPlugin.Log.LogMessage("Disabling AerialFastFall");

        m_harmony.UnpatchSelf();
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
        RefreshPatch();

        if (m_isEnabled)
        {
            FastFallOnPress.LoadConfig(m_configFastFallOnPress);
        }
    }
    
    public override string GetPatchName()
    {
        return "Aerial Fast Fall Patch";
    }
}