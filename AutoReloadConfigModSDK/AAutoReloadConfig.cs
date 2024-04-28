using BepInEx.Unity.IL2CPP;

namespace TheKarters2Mods;

public abstract class AAutoReloadConfig
{
    protected void RegisterToAutoReload()
    {
        AutoReloadConfigModSDKPlugin.Instance.AddToAutoReload(this);
    }
    
    public abstract BasePlugin GetPlugin();

    public abstract void LoadConfig();
}