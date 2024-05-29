using System.Collections.Generic;
using System.Linq;
using TheKartersModdingAssistant;

namespace TheKarters2Mods.Patches.Commands;

public class TwitchCathretCommand_Kill : ITwitchCommand
{
    public bool IsActivated()
    {
        return TwitchCathretCommands.Instance.ConfigKillCommand.Value;
    }

    public bool ShouldExecuteCommand(string _user, string[] _command)
    {
        if (_command.Length < 2)
            return false;
        
        return _command[1].Equals("kill");
    }

    public bool ExecuteCommand(string _user, string[] _command)
    {
        string killParam = _command[2];

        switch (killParam)
        {
            case "pos":
            {
                if (_command.Length != 4)
                    return false;
                
                return KillPlayerAtPos(int.Parse(_command[3]));
            }    
            
            case "humans":
                return KillAllHumans();
            case "ais":
                return KillAllAis();
            
            case "karter":
            {
                if (_command.Length != 4)
                    return false;
                
                return TryKillWithName(_command[3]);
            }    
            
            default:
                return false;
        }
    }

    private bool KillPlayerAtPos(int _karterPos)
    {
        if (_karterPos == 0)
            return false;
        
        List<Player> playersByPosition = Player.GetPlayersSortedByPosition();
        if (playersByPosition.Count < _karterPos)
            return false;
        
        playersByPosition[_karterPos - 1].uHpBarController.Death();
        return true;
    }

    private bool KillAllHumans()
    {
        IEnumerable<Player> allHumans = Player.GetPlayers().Where(_player => _player.IsHuman());
        foreach (Player oneHuman in allHumans)
        {
            oneHuman.uHpBarController.Death();
        }

        return true;
    }

    private bool KillAllAis()
    {
        IEnumerable<Player> allAis = Player.GetPlayers().Where(_player => _player.IsAi());
        foreach (Player oneAi in allAis)
        {
            oneAi.uHpBarController.Death();
        }

        return true;
    }

    private bool TryKillWithName(string _karterName)
    {
        Player player = Player.GetPlayers().Find(_player => _player.GetName().Equals(_karterName));
        if (player == null)
            return false;
        
        player.uHpBarController.Death();
        
        return true;
    }

    public string CommandFeedback(string _user, string[] _command)
    {
        return $"Thanks {_user}, a good day for life";
    }
}