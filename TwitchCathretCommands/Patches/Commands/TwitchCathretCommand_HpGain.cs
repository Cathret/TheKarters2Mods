using System;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches.Commands;

public class TwitchCathretCommand_HpGain : ITwitchCommand
{
    private const int BASE_HP_ADDED = 20;

    public bool IsActivated()
    {
        return TwitchCathretCommands.Instance.ConfigAddHealthCommand.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 3)
            return false;
        
        return _command[1].Equals("hp") && _command[2].Equals("gain");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        int hpAdded = BASE_HP_ADDED;
        if (_command.Length == 4)
        {
            hpAdded = Math.Min(int.Parse(_command[3]), TwitchCathretCommands.Instance.ConfigHealthClampValue.Value);
        }

        Player mainPlayer = Player.FindMainPlayer();
        mainPlayer.SetCurrentHealth(mainPlayer.GetCurrentHealth() + hpAdded);
        
        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, looks like your health helped!";
    }
}