using CounterStrikeSharp.API.Core;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;

namespace TTT.Public.Action;

public class KillAction : Action {
  private readonly Tuple<CCSPlayerController, Role> _actor;
  private readonly Tuple<CCSPlayerController, Role> _attackedPlayer;

  public KillAction(Tuple<CCSPlayerController, Role> actor,
    Tuple<CCSPlayerController, Role> attackedPlayer) {
    _actor          = actor;
    _attackedPlayer = attackedPlayer;
  }

  public override CCSPlayerController Actor() { return _actor.Item1; }

  public override string ActionMessage() {
    var actorRole          = _actor.Item2;
    var attackedPlayerRole = _attackedPlayer.Item2;
    return
      $"[TTT] {actorRole.FormatStringFullAfter(" " + _actor.Item1.PlayerName)}"
      + $" killed {attackedPlayerRole.FormatStringFullAfter(" " + _attackedPlayer.Item1.PlayerName)}. "
      + $"{GoodAction(actorRole, attackedPlayerRole)}";
  }
}