using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.Public.Action;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Logs;
using TTT.Public.Mod.Role;
using TTT.Public.Player;

namespace TTT.Logs;

public class LogsListener(ILogService logService, IPlayerService playerService)
  : IPluginBehavior {
  public void Start(BasePlugin plugin) { }

  [GameEventHandler]
  public HookResult OnPlayerDamage(EventPlayerHurt @event, GameEventInfo info) {
    var attackedPlayer = @event.Userid;

    if (attackedPlayer == null || !attackedPlayer.IsReal())
      return HookResult.Continue;

    var attackedRole = playerService.GetPlayer(attackedPlayer).PlayerRole();

    var attacker = @event.Attacker == null ?
      null :
      new Tuple<CCSPlayerController, Role>(@event.Attacker,
        playerService.GetPlayer(@event.Attacker).PlayerRole());

    logService.AddLog(new DamageAction(attacker,
      new Tuple<CCSPlayerController, Role>(attackedPlayer, attackedRole),
      @event.DmgHealth, ServerExtensions.GetGameRules().RoundTime));

    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info) {
    var killedPlayer = @event.Userid;
    var killer       = @event.Attacker;

    if (killedPlayer == null || !killedPlayer.IsReal())
      return HookResult.Continue;

    var killedRole = playerService.GetPlayer(killedPlayer).PlayerRole();

    if (killer == null || !killer.IsReal()) {
      logService.AddLog(new DeathAction(
        new Tuple<CCSPlayerController, Role>(killedPlayer, killedRole)));
      return HookResult.Continue;
    }

    var killerRole = playerService.GetPlayer(killer).PlayerRole();

    logService.AddLog(new KillAction(
      new Tuple<CCSPlayerController, Role>(killedPlayer, killedRole),
      new Tuple<CCSPlayerController, Role>(killer, killerRole)));
    return HookResult.Continue;
  }
}