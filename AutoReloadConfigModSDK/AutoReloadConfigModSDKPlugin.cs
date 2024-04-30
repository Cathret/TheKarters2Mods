using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Il2CppSystem.Threading;
using String = Il2CppSystem.String;

namespace TheKarters2Mods;

[BepInPlugin(AutoReloadConfigModSDK_BepInExInfo.PLUGIN_GUID, AutoReloadConfigModSDK_BepInExInfo.PLUGIN_NAME, AutoReloadConfigModSDK_BepInExInfo.PLUGIN_VERSION)]
public class AutoReloadConfigModSDKPlugin : BasePlugin
{
    public static AutoReloadConfigModSDKPlugin Instance { get; private set; }

    private readonly Dictionary<string, List<AAutoReloadConfig>> m_autoReloadPathInstance = new();
    
    private FileSystemWatcher m_fileWatcher = null;
    
    private ConfigEntry<float> ConfigMaxTimeBeforeFileAccessFail { get; set; }
    private ConfigEntry<float> ConfigMinTimeBeforeReload { get; set; }
    
    private DateTime m_lastReloadTime = DateTime.Now;

    public void AddToAutoReload(AAutoReloadConfig _oneAAutoReloadConfig)
    {
        string configFilePath = _oneAAutoReloadConfig.GetPlugin().Config.ConfigFilePath;
        if (m_autoReloadPathInstance.TryGetValue(configFilePath, out List<AAutoReloadConfig> outAutoReloadConfigList))
        {
            if (outAutoReloadConfigList.Contains(_oneAAutoReloadConfig))
            {
                Log.LogError($"[{_oneAAutoReloadConfig.GetPatchName()}] is already registered to AutoReload, won't try to register twice");
                return;
            }
            
            outAutoReloadConfigList.Add(_oneAAutoReloadConfig);
        }
        else
        {
            m_autoReloadPathInstance.Add(configFilePath, [_oneAAutoReloadConfig]);
        }
        
        Log.LogMessage($"[{_oneAAutoReloadConfig.GetPatchName()}] registered to AutoReload");
    }
    
    public void RemoveFromAutoReload(AAutoReloadConfig _oneAAutoReloadConfig)
    {
        string configFilePath = _oneAAutoReloadConfig.GetPlugin().Config.ConfigFilePath;
        if (m_autoReloadPathInstance.TryGetValue(configFilePath, out List<AAutoReloadConfig> outAutoReloadConfigList))
        {
            outAutoReloadConfigList.Remove(_oneAAutoReloadConfig);
            
            if (outAutoReloadConfigList.Count == 0)
                m_autoReloadPathInstance.Remove(configFilePath);
            
            Log.LogMessage($"[{_oneAAutoReloadConfig.GetPatchName()}] unregistered from AutoReload");
        }
        else
        {
            Log.LogError($"[{_oneAAutoReloadConfig.GetPatchName()}] could not be unregistered from AutoReload, couldn't find associated config file");
        }
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
            string modsListStr = string.Join(", ", outAutoReloadConfig.Select(_config => _config.GetPatchName()));
            
            if (WaitForFileAccess(filePath))
            {
                Log.LogMessage($"Reloading mods [{modsListStr}]");
                
                outAutoReloadConfig[0].GetPlugin().Config.Reload();
                
                m_lastReloadTime = DateTime.Now;
            }
            else
            {
                Log.LogError($"Couldn't open file [{_e.Name}]. Can't reload patch [{modsListStr}]");
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
