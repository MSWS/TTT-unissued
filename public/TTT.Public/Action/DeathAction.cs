using CounterStrikeSharp.API.Core;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;

namespace TTT.Public.Action;

public class DeathAction : IAction
{
    private readonly Tuple<CCSPlayerController, Role> _actor;

    public DeathAction(Tuple<CCSPlayerController, Role> actor)
    {
        _actor = actor;
    }

    public CCSPlayerController Actor()
    {
        return _actor.Item1;
    }

    public string ActionMessage()
    {
        var actorRole = _actor.Item2;
        return $"[TTT] {actorRole.FormatStringFullAfter(" " + _actor.Item1.PlayerName)}" +
               $" was killed by world.";
    }
}