using System;
using UnityEngine;

namespace TheKarters2Mods.Patches.Injections;

public class TwitchIntegrationSDK_EventManager : MonoBehaviour
{
    public static Action<string, string> onTwitchChatMessage;
    
    public bool ReceiveMessage(string _user, string _message)
    {
        TwitchIntegrationSDKPlugin.Log.LogDebug($"USER: [{_user}] MESSAGE: [{_message}]");
        return HandleMessage(_user, _message);
    }

    private bool HandleMessage(string _user, string _message)
    {
        if (TwitchIntegrationSDK.DebugEnabled)
        {
            if (_message.Equals("PING"))
                TwitchIntegrationSDK.TwitchChatManager.WriteToChat($"PONG {_user}");
        }

        onTwitchChatMessage?.Invoke(_user, _message);
        
        return false;
    }
}