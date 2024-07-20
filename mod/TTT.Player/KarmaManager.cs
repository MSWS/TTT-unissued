using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Player;

public class KarmaManager(IPlayerService playerService) : IPluginBehavior {
  public void Start(BasePlugin plugin) { }

  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info) {
    var killedPlayer = @event.Userid;
    var killer       = @event.Attacker;
    if (killedPlayer == null || !killedPlayer.IsReal())
      return HookResult.Continue;
    if (killer == null || !killer.IsReal()) return HookResult.Continue;

    var gpKiller = playerService.GetPlayer(killer);
    var gpKilled = playerService.GetPlayer(killedPlayer);

    var killerRole = gpKiller.PlayerRole();
    var killedRole = gpKilled.PlayerRole();

    if (killerRole == killedRole) {
      gpKiller.RemoveKarma(5);
      return HookResult.Continue;
    }

    if (killedRole == Role.Traitor) gpKiller.AddKarma(2);
    return HookResult.Continue;
  }
}