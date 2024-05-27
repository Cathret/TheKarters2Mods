using System.Collections.Generic;
using System.Linq;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches.Commands;

public class TwitchCathretCommand_SetReserve : ITwitchCommand
{
    public bool IsActivated()
    {
        return TwitchCathretCommands.Instance.ConfigSetReserveCommand.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 4)
            return false;
        
        return _command[1].Equals("reserve") && _command[2].Equals("set");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        int reserve = int.Parse(_command[3]);
        
        Player.FindMainPlayer().SetCurrentReserveInPercentage(reserve);

        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, let them become SPEED";
    }
}