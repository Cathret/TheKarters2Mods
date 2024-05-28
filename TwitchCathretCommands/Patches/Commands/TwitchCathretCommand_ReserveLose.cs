using System.Collections.Generic;
using System.Linq;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches.Commands;

public class TwitchCathretCommand_ReserveLose : ITwitchCommand
{
    private const int BASE_RESERVE_LOST = 20;

    public bool IsActivated()
    {
        return TwitchCathretCommands.Instance.ConfigRemoveReserveCommand.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 3)
            return false;
        
        return _command[1].Equals("reserve") && _command[2].Equals("lose");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        int reserveLost = BASE_RESERVE_LOST;
        if (_command.Length == 4)
        {
            reserveLost = int.Parse(_command[3]);
        }

        Player mainPlayer = Player.FindMainPlayer();
        mainPlayer.SetCurrentReserveInPercentage(mainPlayer.GetCurrentReserveInPercentage() - reserveLost);

        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, looks like your speed didn't help!";
    }
}