using CounterStrikeSharp.API.Core;

namespace TTT.Public.Action;

public class MiscAction : Action {
  private readonly string _action;
  private readonly CCSPlayerController _actor;

  public MiscAction(string action, CCSPlayerController actor) {
    _action = action;
    _actor  = actor;
  }

  public override CCSPlayerController Actor() { return _actor; }

  public override string ActionMessage() {
    return _actor.PlayerName + " " + _action;
  }
}