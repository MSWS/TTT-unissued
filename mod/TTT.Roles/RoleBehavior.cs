using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Public.Player;
using TTT.Round;

namespace TTT.Roles;

public class RoleBehavior : IRoleService, IPluginBehavior {
  private const int MaxDetectives = 3;
  private readonly IPlayerService service;

  private int innocentsLeft;
  private readonly IRoundService roundService;
  private int traitorsLeft;

  public RoleBehavior(IPlayerService playerService) {
    roundService = new RoundBehavior(this);
    service      = playerService;
  }

  public void Start(BasePlugin parent) {
    ModelHandler.RegisterListener(parent);
    roundService.Start(parent);
  }

  public IRoundService GetRoundService() { return roundService; }

  public void AddRoles() {
    var eligible = Utilities.GetPlayers()
     .Where(player => player.Team is not (CsTeam.Spectator or CsTeam.None))
     .ToList();

    var traitorCount   = (int)Math.Floor(Convert.ToDouble(eligible.Count / 3));
    var detectiveCount = (int)Math.Floor(Convert.ToDouble(eligible.Count / 8));

    traitorsLeft  = traitorCount;
    innocentsLeft = eligible.Count - traitorCount;

    if (detectiveCount > MaxDetectives) detectiveCount = MaxDetectives;

    for (var i = 0; i < traitorCount; i++) {
      var chosen = eligible[Random.Shared.Next(eligible.Count)];
      eligible.Remove(chosen);
      AddTraitor(chosen);
    }

    for (var i = 0; i < detectiveCount; i++) {
      var chosen = eligible[Random.Shared.Next(eligible.Count)];
      eligible.Remove(chosen);
      AddDetective(chosen);
    }

    AddInnocents(eligible.ToArray());
  }

  public ISet<CCSPlayerController> GetTraitors() {
    return GetByRole(Role.Traitor);
  }

  public ISet<CCSPlayerController> GetDetectives() {
    return GetByRole(Role.Detective);
  }

  public ISet<CCSPlayerController> GetInnocents() {
    return GetByRole(Role.Innocent);
  }

  public ISet<CCSPlayerController> GetByRole(Role role) {
    return service.Players()
     .Where(player => player.PlayerRole() == role)
     .Select(player => player.Player())
     .Where(p => p != null)
     .ToHashSet()!;
  }

  public Role GetRole(CCSPlayerController player) {
    return service.GetPlayer(player).PlayerRole();
  }

  public void AddTraitor(params CCSPlayerController[] players) {
    foreach (var player in players) {
      service.GetPlayer(player).SetPlayerRole(Role.Traitor);
      player.SwitchTeam(CsTeam.Spectator);
      player.PrintToCenter(
        Role.Traitor.FormatStringFullBefore("You are now a"));
      player.PrintToChat(Role.Traitor.FormatStringFullBefore("You are now a"));
      ModelHandler.SetModel(player, ModelHandler.ModelPathTmPhoenix);
    }
  }

  public void AddDetective(params CCSPlayerController[] players) {
    foreach (var player in players) {
      service.GetPlayer(player).SetPlayerRole(Role.Detective);
      player.SwitchTeam(CsTeam.CounterTerrorist);
      player.PrintToCenter(
        Role.Detective.FormatStringFullBefore("You are now a"));
      player.GiveNamedItem(CsItem.Taser);
      ModelHandler.SetModel(player, ModelHandler.ModelPathCtmSas);
    }
  }

  public void AddInnocents(params CCSPlayerController[] players) {
    foreach (var player in players) {
      service.GetPlayer(player).SetPlayerRole(Role.Innocent);
      player.PrintToCenter(
        Role.Innocent.FormatStringFullBefore("You are now an"));
      player.SwitchTeam(CsTeam.Spectator);
      ModelHandler.SetModel(player, ModelHandler.ModelPathTmPhoenix);
    }
  }

  public bool IsDetective(CCSPlayerController player) {
    return service.GetPlayer(player).PlayerRole() == Role.Detective;
  }

  public bool IsTraitor(CCSPlayerController player) {
    return service.GetPlayer(player).PlayerRole() == Role.Traitor;
  }

  public void Clear() {
    service.Clr();
    foreach (var key in service.Players()) {
      key.SetPlayerRole(Role.Unassigned);
      if (key.Player() == null) continue;
      RemoveColor(key.Player()!);
    }
  }

  [GameEventHandler]
  public HookResult
    OnRoundStart(EventRoundFreezeEnd @event, GameEventInfo info) {
    roundService.SetRoundStatus(RoundStatus.Waiting);
    foreach (var player in Utilities.GetPlayers()
     .Where(player => player.IsReal() && player.Team != CsTeam.None
        || player.Team != CsTeam.Spectator)) {
      player.RemoveWeapons();
      player.GiveNamedItem("weapon_glock");
      player.GiveNamedItem("weapon_knife");
      service.GetPlayer(player).ModifyKarma();
    }

    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerConnect(EventPlayerConnectFull @event,
    GameEventInfo info) {
    if (Utilities.GetPlayers().Count(player => player.PawnIsAlive) < 3)
      roundService.ForceEnd();

    return HookResult.Continue;
  }

  [GameEventHandler(HookMode.Pre)]
  public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info) {
    info.DontBroadcast = true;

    var playerWhoWasDamaged = @event.Userid;
    var attacker            = @event.Attacker;

    if (playerWhoWasDamaged == null) return HookResult.Continue;

    SetColor(playerWhoWasDamaged);
    playerWhoWasDamaged.ModifyScoreBoard();

    service.GetPlayer(playerWhoWasDamaged).SetKiller(attacker);

    if (IsTraitor(playerWhoWasDamaged)) traitorsLeft--;
    if (IsDetective(playerWhoWasDamaged) || IsInnocent(playerWhoWasDamaged))
      innocentsLeft--;

    if (traitorsLeft == 0 || innocentsLeft == 0)
      Server.NextFrame(() => roundService.ForceEnd());

    // Server.PrintToChatAll(StringUtils.FormatTTT(
    //   $"{GetRole(playerWhoWasDamaged).FormatStringFullAfter(" has been found.")}"));

    if (attacker == playerWhoWasDamaged || attacker == null)
      return HookResult.Continue;

    attacker.ModifyScoreBoard();

    playerWhoWasDamaged.PrintToChat(StringUtils.FormatTTT(
      $"You were killed by {GetRole(attacker).FormatStringFullAfter(" " + attacker.PlayerName)}."));
    attacker.PrintToChat(StringUtils.FormatTTT(
      $"You killed {GetRole(playerWhoWasDamaged).FormatStringFullAfter(" " + playerWhoWasDamaged.PlayerName)}."));

    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info) {
    var players = Utilities.GetPlayers()
     .Where(player => player.IsValid)
     .ToList();

    foreach (var player in players)
      player.PrintToCenter(GetWinner().FormatStringFullAfter("s has won!"));

    Clear();
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event,
    GameEventInfo info) {
    var player = @event.Userid;
    if (player == null) return HookResult.Continue;
    service.RemovePlayer(player);
    if (service.Players().Count == 0)
      roundService.SetRoundStatus(RoundStatus.Paused);

    return HookResult.Continue;
  }

  public bool IsInnocent(CCSPlayerController player) {
    return service.GetPlayer(player).PlayerRole() == Role.Innocent;
  }

  private Role GetWinner() {
    return traitorsLeft == 0 ? Role.Traitor : Role.Innocent;
  }

  public void SetColor(CCSPlayerController player) {
    if (!player.IsReal()) return;

    var pawn = player.PlayerPawn.Value;

    if (pawn == null || !pawn.IsValid) return;

    pawn.RenderMode = RenderMode_t.kRenderTransColor;
    pawn.Render     = GetRole(player).GetColor();

    Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
  }

  public void RemoveColor(CCSPlayerController player) {
    if (!player.IsReal()) return;

    var pawn = player.PlayerPawn.Value;

    if (pawn == null || !pawn.IsValid) return;

    pawn.RenderMode = RenderMode_t.kRenderTransColor;
    pawn.Render     = Color.FromArgb(254, 255, 255, 255);


    Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
  }
}