using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;

namespace TTT.Round;

public class RoundBehavior(IRoleService roleService) : IRoundService {
  private Round? round;
  private int roundId = 1;
  private RoundStatus roundStatus = RoundStatus.Paused;

  public void Start(BasePlugin plugin) {
    plugin.RegisterListener<Listeners.OnTick>(TickWaiting);
    plugin.AddCommandListener("jointeam", OnTeamJoin);
    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(BlockDamage,
      HookMode.Pre);
    plugin.AddTimer(3, EndRound, TimerFlags.REPEAT);
  }

  public RoundStatus GetRoundStatus() { return roundStatus; }

  public void SetRoundStatus(RoundStatus roundStatus) {
    switch (roundStatus) {
      case RoundStatus.Ended:
        ForceEnd();
        break;
      case RoundStatus.Waiting:
        round = new Round(roleService, null, roundId);
        break;
      case RoundStatus.Started:
        ForceStart();
        break;
      case RoundStatus.Paused:
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(roundStatus), roundStatus,
          "Invalid round status.");
    }

    this.roundStatus = roundStatus;
  }

  public void TickWaiting() {
    if (round == null) {
      round = new Round(roleService, null, roundId);
      return;
    }

    if (roundStatus != RoundStatus.Waiting) return;

    round.Tick();

    if (round.GraceTime() != 0) return;


    if (Utilities.GetPlayers()
     .Where(player => player is { IsValid: true, PawnIsAlive: true })
     .ToList()
     .Count <= 2) {
      Server.PrintToChatAll(StringUtils.FormatTTT(
        "Not enough players to start the round. Round has been ended."));
      roundStatus = RoundStatus.Paused;
      return;
    }

    SetRoundStatus(RoundStatus.Started);
  }

  public void ForceStart() {
    foreach (var player in Utilities.GetPlayers().ToList())
      player.VoiceFlags = VoiceFlags.Normal;
    round!.Start();
    ServerExtensions.GetGameRules().RoundTime = 360;
    Utilities.SetStateChanged(ServerExtensions.GetGameRulesProxy(),
      "CCSGameRulesProxy", "m_pGameRules");
  }

  public void ForceEnd() {
    if (roundStatus == RoundStatus.Ended) return;
    roundStatus = RoundStatus.Ended;
    Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
     .First()
     .GameRules!.TerminateRound(5, RoundEndReason.RoundDraw);
  }

  private void EndRound() {
    if (roundStatus == RoundStatus.Started
      && Utilities.GetPlayers().Count(player => player.PawnIsAlive) == 1)
      ForceEnd();

    var traitorCount =
      roleService.GetTraitors().Count(player => player.PawnIsAlive);
    var innocentCount =
      roleService.GetInnocents().Count(player => player.PawnIsAlive);
    var detectiveCount = roleService.GetDetectives()
     .Count(player => player.PawnIsAlive);

    if (roundStatus == RoundStatus.Started
      && (traitorCount == 0 || innocentCount + detectiveCount == 0))
      ForceEnd();
  }

  private HookResult BlockDamage(DynamicHook hook) {
    if (hook.GetParam<CEntityInstance>(0).DesignerName is not "player")
      return HookResult.Continue;
    return roundStatus != RoundStatus.Waiting ?
      HookResult.Continue :
      HookResult.Stop;
  }

  private HookResult
    OnTeamJoin(CCSPlayerController? executor, CommandInfo info) {
    if (roundStatus != RoundStatus.Started) return HookResult.Continue;
    if (executor == null) return HookResult.Continue;
    if (!executor.IsReal()) return HookResult.Continue;
    if (roleService.GetRole(executor) != Role.Unassigned)
      return HookResult.Continue;
    Server.NextFrame(() => executor.CommitSuicide(false, true));

    return HookResult.Continue;
  }

  private HookResult OnRoundEnd(EventRoundEnd _, GameEventInfo __) {
    roundId++;
    return HookResult.Continue;
  }
}