using System;
using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods;

public abstract class AAutoReloadConfig
{
    private bool m_isRegistered = false;
    
    protected bool RegisterToAutoReload()
    {
        BasePlugin plugin = GetPlugin();
        if (plugin == null)
            return false;

        if (m_isRegistered)
        {
            plugin.Log.LogError($"{GetType().Name} instance already registered to AutoReload, will not register twice");
            return false;
        }

        plugin.Config.ConfigReloaded += Internal_LoadConfig;

        AutoReloadConfigModSDKPlugin autoReloadPlugin = AutoReloadConfigModSDKPlugin.Instance;
        if (autoReloadPlugin == null)
        {
            plugin.Log.LogError($"Can't find Instance of AutoReloadModSDK Plugin. {GetType().Name} couldn't be registered to AutoReload");
            return false;
        }
        
        autoReloadPlugin.AddToAutoReload(this);

        m_isRegistered = true;

        return true;
    }

    protected bool TryUnregisterFromAutoReload()
    {
        if (!m_isRegistered)
            return false;
        
        BasePlugin plugin = GetPlugin();
        if (plugin != null)
        {
            plugin.Config.ConfigReloaded -= Internal_LoadConfig;

            AutoReloadConfigModSDKPlugin.Instance?.RemoveFromAutoReload(this);
        }

        m_isRegistered = false;

        return true;
    }

    ~AAutoReloadConfig()
    {
        TryUnregisterFromAutoReload();
    }

    private void Internal_LoadConfig(object _sender, EventArgs _e)
    {
        LoadConfig();
    }
    
    public abstract BasePlugin GetPlugin();

    protected abstract void LoadConfig();

    public virtual string GetPatchName()
    {
        return GetType().Name;
    }
}