using System.Collections.Generic;
using System.Linq;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches.Commands;

public class TwitchCathretCommand_HpLose : ITwitchCommand
{
    private const int BASE_HP_LOST = 20;

    public bool IsActivated()
    {
        return TwitchCathretCommands.Instance.ConfigRemoveHealthCommand.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 3)
            return false;
        
        return _command[1].Equals("hp") && _command[2].Equals("lose");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        int hpLost = BASE_HP_LOST;
        if (_command.Length == 4)
        {
            hpLost = int.Parse(_command[3]);
        }

        Player mainPlayer = Player.FindMainPlayer();
        mainPlayer.SetCurrentHealth(mainPlayer.GetCurrentHealth() - hpLost);
        
        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, looks like your health didn't help!";
    }
}