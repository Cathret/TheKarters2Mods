using System;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches.Commands;

public class TwitchCathretCommand_ReserveGain : ITwitchCommand
{
    private const int BASE_RESERVE_ADDED = 20;

    public bool IsActivated()
    {
        return TwitchCathretCommands.Instance.ConfigAddReserveCommand.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 3)
            return false;
        
        return _command[1].Equals("reserve") && _command[2].Equals("gain");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        int reserveAdded = BASE_RESERVE_ADDED;
        if (_command.Length == 4)
        {
            reserveAdded = Math.Min(int.Parse(_command[3]), TwitchCathretCommands.Instance.ConfigReserveClampValue.Value);
        }

        Player mainPlayer = Player.FindMainPlayer();
        mainPlayer.SetCurrentReserveInPercentage(mainPlayer.GetCurrentReserveInPercentage() + reserveAdded);

        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, looks like your speed helped!";
    }
}