using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Behaviors;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Roles.Shop;

public class CreditManager(IPlayerService playerService) : IPluginBehavior {
  public void Start(BasePlugin plugin) { }


  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info) {
    var attacker = @event.Attacker;
    var victim   = @event.Userid;

    if (attacker == null || victim == null) return HookResult.Continue;
    if (attacker == victim) return HookResult.Continue;

    var attackerPlayer = playerService.GetPlayer(attacker);
    var victimPlayer   = playerService.GetPlayer(victim);

    if (attackerPlayer.PlayerRole() == Role.Traitor
      && victimPlayer.PlayerRole() != Role.Traitor) {
      attackerPlayer.AddCredits(250);
      return HookResult.Continue;
    }

    if (attackerPlayer.PlayerRole() != Role.Traitor
      && victimPlayer.PlayerRole() == Role.Traitor) {
      attackerPlayer.AddCredits(250);
      return HookResult.Continue;
    }

    attackerPlayer.RemoveCredits(100);

    return HookResult.Continue;
  }
}