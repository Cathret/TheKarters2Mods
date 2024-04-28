using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods;

[BepInPlugin(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID, AutoReloadConfigModSDK_BepInExInfo.PLUGIN_NAME, AutoReloadConfigModSDK_BepInExInfo.PLUGIN_VERSION)]
public class AutoReloadConfigModSDKPlugin : BasePlugin
{
    public static AutoReloadConfigModSDKPlugin Instance { get; private set; }

    private FileSystemWatcher m_fileWatcher = null;
    
    private readonly Dictionary<string, AAutoReloadConfig> m_autoReloadPathInstance = new();

    public void AddToAutoReload(AAutoReloadConfig _oneAAutoReloadConfig)
    {
        m_autoReloadPathInstance.Add(_oneAAutoReloadConfig.GetPlugin().Config.ConfigFilePath, _oneAAutoReloadConfig);
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
        Log.LogMessage($"Config File modified [{_e.Name}]");

        string filePath = _e.FullPath;
        if (m_autoReloadPathInstance.TryGetValue(filePath, out var outAutoReloadConfig))
        {
            Log.LogMessage($"Reloading mod [{outAutoReloadConfig.GetType().Name}]");

            outAutoReloadConfig.LoadConfig();
        }
    }
}
