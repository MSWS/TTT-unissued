using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Public.Player;

namespace TTT.Manager;

public class InfoManager(IPlayerService playerService, IRoleService manager)
  : IPluginBehavior {
  private readonly IRoundService manager = manager.GetRoundService();

  private readonly
    Dictionary<CCSPlayerController, Tuple<CCSPlayerController, Role>>
    playerLookAtRole = new();

  private readonly Dictionary<CCSPlayerController, Tuple<string, Role>>
    spectatorLookAtRole = new();

  public void Start(BasePlugin plugin) {
    plugin.RegisterListener<Listeners.OnTick>(OnTick);
    plugin.AddTimer(0.1f, OnTickAll, TimerFlags.REPEAT);
  }

  public void Reset() {
    playerLookAtRole.Clear();
    spectatorLookAtRole.Clear();
  }

  public void RegisterLookAtRole(CCSPlayerController player,
    Tuple<CCSPlayerController, Role> role) {
    playerLookAtRole.TryAdd(player, role);
  }

  public void RemoveLookAtRole(CCSPlayerController player) {
    playerLookAtRole.Remove(player);
  }

  public void OnTick() {
    if (manager.GetRoundStatus() != RoundStatus.Started) return;
    foreach (var gamePlayer in playerService.Players()) {
      var player = gamePlayer.Player();

      if (player == null) continue;
      if (!player.IsReal()) continue;

      var playerRole = gamePlayer.PlayerRole();

      if (playerRole == Role.Unassigned) continue;

      if (!player.PawnIsAlive) continue;

      if (!playerLookAtRole.TryGetValue(player, out var value)) {
        player.PrintToCenterHtml(
          $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()}");
        continue;
      }

      if (!value.Item1.IsReal()) continue;

      if (!value.Item1.PawnIsAlive)
        HandleDeadTarget(player, playerRole, value.Item1, value.Item2);
      else
        HandleAliveTarget(player, playerRole, value.Item1, value.Item2);
    }
  }

  public void
    HandleDeadTarget(CCSPlayerController player, Role playerRole,
      CCSPlayerController target, Role targetRole) {
    player.PrintToCenterHtml(
      $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
      + $"<font class='fontsize=l' color='red'>{target.PlayerName}'s Corpse <br>"
      + $"<font class='fontsize=m' color='yellow'>{target.PlayerName}'s Role: {targetRole.GetCenterRole()}");
  }

  public void HandleAliveTarget(CCSPlayerController player, Role playerRole,
    CCSPlayerController target, Role targetRole) {
    switch (targetRole) {
      case Role.Detective:
        player.PrintToCenterHtml(
          $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
          + $"<font class='fontsize=m' color='yellow'>{target.PlayerName}'s Role: {targetRole.GetCenterRole()}");
        return;
      case Role.Traitor when playerRole == Role.Traitor:
        player.PrintToCenterHtml(
          $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
          + $"<font class='fontsize=m' color='maroon'>{target.PlayerName}'s Role: {targetRole.GetCenterRole()}");
        return;
      default:
        Server.NextFrame(() => player.PrintToCenterHtml(
          $"<font class='fontsize=m' color='yellow'>Your Role: {playerRole.GetCenterRole()} <br>"
          + $"<font class='fontsize=m' color='yellow'>{target.PlayerName}'s Role: {Role.Unassigned.GetCenterRole()}"));
        break;
    }
  }

  public void OnTickAll() {
    var players = playerService.Players()
     .Select(plr => plr.Player())
     .Where(p => p != null && p.IsReal())
     .ToList();

    playerLookAtRole.Clear();

    foreach (var player in players) {
      if (player == null) continue;
      if (!player.IsReal()) continue;

      var target = player.GetClientPlayerAimTarget();

      if (target == null || !target.IsReal()) continue;
      if (!target.IsReal()) continue;

      RegisterLookAtRole(player,
        new Tuple<CCSPlayerController, Role>(target,
          playerService.GetPlayer(target).PlayerRole()));
    }
  }

  [GameEventHandler]
  private HookResult OnPlayerSpectateChange(EventSpecTargetUpdated @event,
    GameEventInfo info) {
    var player = @event.Userid;
    var target = new CCSPlayerController(@event.Target);

    if (player == null || !player.IsReal() || !target.IsReal())
      return HookResult.Continue;

    spectatorLookAtRole.TryAdd(player,
      new Tuple<string, Role>(target.PlayerName,
        playerService.GetPlayer(target).PlayerRole()));

    return HookResult.Continue;
  }
}