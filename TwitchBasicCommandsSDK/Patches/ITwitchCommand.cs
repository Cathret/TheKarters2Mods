namespace TheKarters2Mods.Patches;

public interface ITwitchCommand
{
    // _command[0] will always be the prefix
    
    public bool IsActivated();
    public bool ShouldExecuteCommand(string _user, string[] _command);
    public bool ExecuteCommand(string _user, string[] _command);
    public string CommandFeedback(string _user, string[] _command);
}