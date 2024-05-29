# Twitch Basic Commands SDK v1.0.0

## Description
**Base Twitch Basic Commands SDK** that can be used to **react to basic commands** with prefix and space-separated commands.

Can **inherit from interface** to be able to **react to Twitch command**.

## Dependency
- **Disable Leaderboards** v1.0.0
- **Auto Reload Config Mod SDK** v1.0.0
- **Twitch Integration SDK** v1.0.0

## Using the SDK

### How it works
When a chat is sent starting **with the Prefix set in Config**, the message is processed and goes through all the Commands.

It will call `ShouldExecuteCommand()` and `ExecuteCommand()`. If the Command is executed, it will send a callback message on the Chat, and reset the Cooldown.

The Cooldown set in Config is **the time in seconds between two Commands** can be processed.

There are **no buffering**, the first Command sent after the Cooldown will be executed.

### Creating your Commands
To create a new Basic Command, you will need to inherit from `ITwitchCommand` and implement its methods.

```C#
public class BasicCommand_YOURCOMMAND : ITwitchCommand
{
    public bool IsActivated()
    {
        return YOUR_PLUGIN.Instance.ConfigYOURCOMMAND.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 2)
            return false;
        
        return _command[1].Equals("COMMAND");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        ...
        
        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, looks like your COMMAND WAS CALLED!";
    }
}
```

`IsActivated()` is **not called by the SDK**, but is here to be able to have a single method for Configurations.

`ShouldExecuteCommand()` should tests the "initial format" of the command. It should return **true if we want to try to execute this command**, false otherwise.

`ExecuteCommand()` will **try to execute the command**. If it manages to execute it, it should return true, false otherwise.

`CommandFeedback()` returns the **text that will be sent to the Twitch Chat after a Command is executed**.

### Registering your Commands
To be able to **execute your commands**, you simply need to register them using the `TwitchBasicCommandsSDK.Instance.RegisterCommand()` function.

```C#
private void Init()
{
    m_myCommand = new MyCommand();
}

private void EnableCommand()
{
    if (m_myCommand.IsActivated())
        TwitchBasicCommandsSDK.Instance.RegisterCommand(m_myCommand);
}

private void DisableCommand()
{
    TwitchBasicCommandsSDK.Instance.UnregisterCommand(m_myCommand);
}
```

To be able to call this function, you will **need to have an instance of your Command** to send as parameter.

### Automatically register all the Commands from your Plugin
You can use this template to **automatically create an Instance of all your Commands** inheriting from `ITwitchCommand` and **register them** if their `IsActivated()` function returns true.

```C#
private void EnableActivated()
{
    foreach (ITwitchCommand oneCommand in m_allCathretCommandsInstances)
    {
        if (oneCommand.IsActivated())
        {
            TwitchBasicCommandsSDK.Instance.RegisterCommand(oneCommand);
        }
    }
    
    TwitchCathretCommandsPlugin.Log.LogDebug($"Twitch Cathret Commands Enabled");
}

private void DisableAll()
{
    foreach (ITwitchCommand oneCommand in m_allCathretCommandsInstances)
    {
        TwitchBasicCommandsSDK.Instance.UnregisterCommand(oneCommand);
    }
    
    TwitchCathretCommandsPlugin.Log.LogDebug($"Twitch Cathret Commands Disabled");
}

private static readonly IEnumerable<Type> m_allCathretCommandsTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
    .Where(_p => typeof(ITwitchCommand).IsAssignableFrom(_p) && _p != typeof(ITwitchCommand));

private readonly List<ITwitchCommand> m_allCathretCommandsInstances =
    m_allCathretCommandsTypes.Select(_type => (ITwitchCommand)Activator.CreateInstance(_type)).ToList();
```

## Parameters

### Global
#### Minimum Time Between Interactions
**In Seconds**. **Minimum time between two interactions** from Twitch Chat.

#### Command Prefix
**Prefix** of the commands to be used by this Twitch Interaction Bot.
