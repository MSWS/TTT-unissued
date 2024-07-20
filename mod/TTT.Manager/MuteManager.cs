using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Manager;

namespace TTT.Manager;

public class MuteManager : IMuteService, IPluginBehavior {
  public void Mute(CCSPlayerController player) {
    if (BypassMute(player)) return;

    player.VoiceFlags |= VoiceFlags.Muted;
  }

  public void UnMute(CCSPlayerController player) {
    player.VoiceFlags &= ~VoiceFlags.Muted;
  }

  public void UnMuteAll() {
    foreach (var player in Utilities.GetPlayers()
     .Where(player => player.IsReal() && player.Team == CsTeam.Terrorist))
      UnMute(player);
    foreach (var player in Utilities.GetPlayers()
     .Where(player
        => player.IsReal() && player.Team == CsTeam.CounterTerrorist))
      UnMute(player);
  }

  public void MuteAll() {
    foreach (var player in Utilities.GetPlayers()
     .Where(player => player.IsReal() && player.Team == CsTeam.Terrorist))
      Mute(player);
    foreach (var player in Utilities.GetPlayers()
     .Where(player
        => player.IsReal() && player.Team == CsTeam.CounterTerrorist))
      Mute(player);
  }

  public void Start(BasePlugin plugin) {
    plugin.RegisterListener<Listeners.OnClientVoice>(OnPlayerSpeak);
  }

  [GameEventHandler]
  public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info) {
    if (@event.Userid == null) return HookResult.Continue;
    Mute(@event.Userid);
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info) {
    UnMuteAll();
    return HookResult.Continue;
  }

  private void OnPlayerSpeak(int playerSlot) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    if (!player.IsReal()) return;

    if (!player.PawnIsAlive && !BypassMute(player)) {
      // Normal players can't speak when dead
      Mute(player);
      Server.NextFrame(() => player.PrintToCenter("You are dead and muted!"));
      return;
    }

    if (IsMuted(player)) {
      Server.NextFrame(() => player.PrintToCenter("You are muted!"));
    }
  }

  private bool IsMuted(CCSPlayerController player) {
    if (!player.IsReal()) return false;
    return (player.VoiceFlags & VoiceFlags.Muted) != 0;
  }

  private bool BypassMute(CCSPlayerController player) {
    return player.IsReal()
      && AdminManager.PlayerHasPermissions(player, "@css/chat");
  }
}