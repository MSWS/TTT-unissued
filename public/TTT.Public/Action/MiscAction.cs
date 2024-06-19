using CounterStrikeSharp.API.Core;

namespace TTT.Public.Action;

public class MiscAction : IAction
{
    private readonly string _action;
    private readonly CCSPlayerController _actor;

    public MiscAction(string action, CCSPlayerController actor)
    {
        _action = action;
        _actor = actor;
    }

    public CCSPlayerController Actor()
    {
        return _actor;
    }

    public string ActionMessage()
    {
        return _actor.PlayerName + " " +  _action;
    }
}