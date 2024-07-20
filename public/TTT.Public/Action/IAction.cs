using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Role;

namespace TTT.Public.Action;

public abstract class Action {
  public abstract CCSPlayerController Actor();
  public abstract string ActionMessage();

  protected static string GoodAction(Role actor, Role actor2) {
    return actor == actor2 ? "[Bad action!]" : "";
  }
}