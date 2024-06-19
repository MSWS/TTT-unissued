using CounterStrikeSharp.API.Core;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;

namespace TTT.Public.Action;

public class DamageAction : IAction
{
    private readonly Tuple<CCSPlayerController, Role> _actor;
    private readonly Tuple<CCSPlayerController, Role> _attackedPlayer;
    private readonly int _damage;

    public DamageAction(Tuple<CCSPlayerController, Role> actor, Tuple<CCSPlayerController, Role> attackedPlayer,
        int damage, int roundTime)
    {
        _actor = actor;
        _attackedPlayer = attackedPlayer;
        _damage = damage;
    }

    public CCSPlayerController Actor()
    {
        return _actor.Item1;
    }

    public string ActionMessage()
    {
        var actorRole = _actor.Item2;
        var attackedPlayerRole = _attackedPlayer.Item2;
        return $"[TTT] {actorRole.FormatStringFullAfter(" " + _actor.Item1.PlayerName)}" +
               $" damaged {attackedPlayerRole.FormatStringFullAfter(" " + _attackedPlayer.Item1.PlayerName)}" +
               $" for {_damage} hp. {IAction.GoodAction(actorRole, attackedPlayerRole)}";
    }
}