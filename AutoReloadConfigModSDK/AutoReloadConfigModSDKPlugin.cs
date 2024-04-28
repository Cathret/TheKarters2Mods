using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Il2CppSystem.Threading;

namespace TheKarters2Mods;

[BepInPlugin(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID, AutoReloadConfigModSDK_BepInExInfo.PLUGIN_NAME, AutoReloadConfigModSDK_BepInExInfo.PLUGIN_VERSION)]
public class AutoReloadConfigModSDKPlugin : BasePlugin
{
    public static AutoReloadConfigModSDKPlugin Instance { get; private set; }

    private readonly Dictionary<string, AAutoReloadConfig> m_autoReloadPathInstance = new();
    
    private FileSystemWatcher m_fileWatcher = null;
    
    private ConfigEntry<float> ConfigMaxTimeBeforeFileAccessFail { get; set; }
    private ConfigEntry<float> ConfigMinTimeBeforeReload { get; set; }
    
    private DateTime m_lastReloadTime = DateTime.Now;

    public void AddToAutoReload(AAutoReloadConfig _oneAAutoReloadConfig)
    {
        m_autoReloadPathInstance.Add(_oneAAutoReloadConfig.GetPlugin().Config.ConfigFilePath, _oneAAutoReloadConfig);
        Log.LogMessage($"{_oneAAutoReloadConfig.GetType().Name} registered to AutoReload");
    }
    
    public void RemoveFromAutoReload(AAutoReloadConfig _oneAAutoReloadConfig)
    {
        m_autoReloadPathInstance.Remove(_oneAAutoReloadConfig.GetPlugin().Config.ConfigFilePath);
        Log.LogMessage($"{_oneAAutoReloadConfig.GetType().Name} unregistered to AutoReload");
    }

    private void SetupConfigFilesWatcher()
    {
        m_fileWatcher = new FileSystemWatcher(Paths.ConfigPath);

        m_fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
        
        m_fileWatcher.Filter = "*.cfg";
        m_fileWatcher.IncludeSubdirectories = true;
        m_fileWatcher.EnableRaisingEvents = true;
        
        m_fileWatcher.Changed += OnConfigFileModified;
    }
    
    public override void Load()
    {
        Instance = this;
        
        Log.LogMessage($"Plugin {AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID} ({AutoReloadConfigModSDK_BepInExInfo.PLUGIN_NAME}) is loaded!");
        
        ConfigMaxTimeBeforeFileAccessFail = Config.Bind("AutoReloadConfigModSDK", "MaxTimeBeforeFileAccessFail", 3f, new ConfigDescription("Maximum time to wait trying to get access to config files before failing. In Seconds.", new AcceptableValueRange<float>(1f, 10f)));
        ConfigMinTimeBeforeReload = Config.Bind("AutoReloadConfigModSDK", "MinTimeBeforeReload", 1f, new ConfigDescription("Minimum global time before a new auto-reload can be made. In Seconds.", new AcceptableValueRange<float>(0.5f, 10f)));

        SetupConfigFilesWatcher();
    }

    public override bool Unload()
    {
        m_fileWatcher.Changed -= OnConfigFileModified;

        m_fileWatcher.Dispose();
        
        return base.Unload();
    }

    private void OnConfigFileModified(object _sender, FileSystemEventArgs _e)
    {
        if (_e.ChangeType != WatcherChangeTypes.Changed)
            return;

        TimeSpan timeSinceLastReload = DateTime.Now - m_lastReloadTime;
        if (timeSinceLastReload.Seconds < ConfigMinTimeBeforeReload.Value)
            return;
        
        Log.LogMessage($"Config File modified [{_e.Name}]");

        string filePath = _e.FullPath;
        if (m_autoReloadPathInstance.TryGetValue(filePath, out var outAutoReloadConfig))
        {
            Log.LogMessage($"Reloading mod [{outAutoReloadConfig.GetType().Name}]");

            if (WaitForFileAccess(filePath))
            {
                outAutoReloadConfig.GetPlugin().Config.Reload();;
                
                m_lastReloadTime = DateTime.Now;
            }
            else
            {
                Log.LogError($"Couldn't open file [{_e.Name}], couldn't reload Plugin [{outAutoReloadConfig.GetType().Name}]");
            }
        }
    }

    private bool WaitForFileAccess(string _filePath)
    {
        bool hasFileAccess = false;
        DateTime firstAccessTry = DateTime.Now;
        float maxSecondsBeforeFail = ConfigMaxTimeBeforeFileAccessFail.Value;
        
        while (!hasFileAccess)
        {
            try
            {
                File.Open(_filePath, FileMode.Open, FileAccess.Read).Dispose();
                hasFileAccess = true;
            }
            catch (IOException)
            {
                TimeSpan timeSinceFirstAccess = DateTime.Now - firstAccessTry;
                if (timeSinceFirstAccess.Seconds > maxSecondsBeforeFail)
                    return false;
            }
        }

        return true;
    }
}
