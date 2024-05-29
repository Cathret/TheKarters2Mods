# Twitch Integration SDK v1.0.0

## Description
**Base Twitch SDK** that can be used to **interact with Twitch Chat**. It needs some configuration.

Can be based of to be able to react to Twitch message and send Twitch message.

## Dependency
- **Disable Leaderboards** v1.0.0
- **Auto Reload Config Mod SDK** v1.0.0

## Using the SDK

### Setting up the OAuth Token
For the SDK to have the Authorization to read and write messages on the chat, you need to **Generate a Token**.

To generate this token, you must be connected to Twitch and authorize it using [this link](https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=es3ykecmg5vhdvu2loirooeq7tenkn&redirect_uri=http://localhost:3000&scope=chat%3Aedit%20chat%3Aread).

After authorization, you will be redirected to an empty page. It's **NORMAL**, as you wish to **retrieve your Token in the URL of this page**, such as `...token={yourToken}&`.

**/!\ DO NOT SHARE THIS TOKEN /!\\**. It permits any bot to access your chat with read and write access using your name.

Once the Token is retrieved, you can **fill the Configuration file** with correct value.

For now, `Username = TestTKIntegration`.
For the Token you retrieved, `OAuth Token = {yourToken}`.
For the Channel name, your channel name LOWERCASE, `Channel Name = {channelnamelowercase}`.

### Receiving Message
To be able to **react to messages sent in the Chat**, you simply have to bind a function on the `TwitchIntegrationSDK.OnTwitchChatMessage` callback.
```C#
private void Init()
{
    TwitchIntegrationSDK.OnTwitchChatMessage += OnTwitchChatMessage;
}

private void Destroy()
{
    TwitchIntegrationSDK.OnTwitchChatMessage -= OnTwitchChatMessage;
}

private void OnTwitchChatMessage(string _user, string _message)
{
    ...
}
```

### Sending Message
**Sending a message** is pretty straight forward, you can use `TwitchIntegrationSDK.TwitchChatManager.WriteToChat()` function.
```C#
private void OnTwitchChatMessage(string _user, string _message)
{
    ...
    
    string feedbackMessage = "Twitch Message Received";
    TwitchIntegrationSDK.TwitchChatManager.WriteToChat(feedbackMessage);
}
```

## Parameters

### Global
#### Username
~~Username of the Bot in Twitch Chat.~~ Use **TestTKIntegration**.

#### OAuth Token
/!\ DO NOT SHARE THIS INFORMATION /!\\\
OAuth Token of the Authorization given on your Twitch Account.

#### Channel Name
Channel name of the Twitch Channel to connect to.
