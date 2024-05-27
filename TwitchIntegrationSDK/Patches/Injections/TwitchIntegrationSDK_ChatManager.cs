using System;
using System.IO;
using System.Net.Sockets;
using BepInEx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheKarters2Mods.Patches.Injections;

public class TwitchIntegrationSDK_ChatManager : MonoBehaviour
{
    private TcpClient m_twitchClient = null;
    private StreamReader m_reader = null;
    private StreamWriter m_writer = null;
    
    public void OnEnable()
    {
        TwitchIntegrationSDKPlugin.Log.LogInfo("Twitch Integration Component AWAKE");

        TryConnectToTwitch();

        if (m_twitchClient != null && m_twitchClient.Connected)
        {
            TwitchIntegrationSDKPlugin.Log.LogInfo("TwitchIntegrationComponent.Awake() - Connected!");
            return;
        }
        
        TwitchIntegrationSDKPlugin.Log.LogWarning("TwitchIntegrationComponent.Awake() - could not connect to chat, destroying instance!");
        enabled = false;
        TwitchIntegrationSDK.UnPatch();
    }

    public void Update()
    {
        if (!m_twitchClient.Connected)
            TryConnectToTwitch();
        ReadChat();
    }

    private bool TryConnectToTwitch()
    {
        m_twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        
        m_reader = new StreamReader(m_twitchClient.GetStream());
        m_writer = new StreamWriter(m_twitchClient.GetStream());
        
        string passMsg = $"PASS oauth:{TwitchIntegrationSDK.TwitchOAuthToken}";
        string nickMsg = $"NICK {TwitchIntegrationSDK.TwitchUserName}";
        string joinMsg = $"JOIN #{TwitchIntegrationSDK.TwitchChannelName}";
        
        TwitchIntegrationSDKPlugin.Log.LogDebug($"MESSAGE SENT: \"{passMsg}\"");
        TwitchIntegrationSDKPlugin.Log.LogDebug($"MESSAGE SENT (DO NOT SHARE): \"{nickMsg}\"");
        TwitchIntegrationSDKPlugin.Log.LogDebug($"MESSAGE SENT: \"{joinMsg}\"");
        
        m_writer.WriteLine(passMsg);
        m_writer.WriteLine(nickMsg);
        m_writer.WriteLine(joinMsg);
        
        m_writer.Flush();
        
        return true;
    }

    private bool ReadChat()
    {
        if (m_twitchClient.Available <= 0)
            return false;

        TwitchIntegrationSDK_EventManager eventManager = TwitchIntegrationSDK.TwitchEventManager;
        if (!eventManager)
            return false;

        string readMessage = m_reader.ReadLine();
        if (readMessage.IsNullOrWhiteSpace() || !readMessage.Contains("PRIVMSG"))
        {
            TwitchIntegrationSDKPlugin.Log.LogDebug($"FULL MESSAGE: \"{readMessage}\"");
            return false;
        }
        
        string user = readMessage.Substring(0, readMessage.IndexOf("!", 1, StringComparison.Ordinal)).Substring(1);
        string message = readMessage.Substring(readMessage.IndexOf(":", 1, StringComparison.Ordinal) + 1);
        
        TwitchIntegrationSDKPlugin.Log.LogInfo($"({user}: {message})");
        
        eventManager.ReceiveMessage(user, message);

        return true;
    }

    public bool WriteToChat(string _message, bool _flushAfter = true)
    {
        if (!m_twitchClient.Connected)
            return false;

        string ircMessage = $"PRIVMSG #{TwitchIntegrationSDK.TwitchChannelName} : |TK2| {_message}";
        
        TwitchIntegrationSDKPlugin.Log.LogDebug($"SENDING MESSAGE ON CHAT: {ircMessage})");

        m_writer.WriteLine(ircMessage);
        
        if (_flushAfter)
            m_writer.Flush();

        return true;
    }
}