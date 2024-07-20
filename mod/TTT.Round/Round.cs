using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;

namespace TTT.Round;

public class Round(IRoleService roleService, RoundConfig? config, int roundId) {
  private readonly RoundConfig? config = config;
  private float graceTime = 20 * 64;

  public void Tick() {
    graceTime--;

    var players = Utilities.GetPlayers()
     .Where(player => player.IsValid)
     .Where(player => player.IsReal())
     .ToList();

    var formattedColor =
      $"<font color=\"#{Color.Green.R:X2}{Color.Green.G:X2}{Color.Green.B:X2}\">";

    foreach (var player in players)
      Server.NextFrame(() => {
        player.PrintToCenterHtml(
          $"{formattedColor}<b>[TTT] Game is starting in {Math.Floor(graceTime / 64)} seconds</b></font>");
      });
  }

  public float GraceTime() { return graceTime; }

  public void Start() {
    foreach (var player in Utilities.GetPlayers()
     .Where(player => !player.PawnIsAlive)
     .Where(player
        => player.Team is CsTeam.Terrorist or CsTeam.CounterTerrorist))
      player.Respawn();

    roleService.AddRoles();
    Server.NextFrame(()
      => Server.PrintToChatAll(StringUtils.FormatTTT(
        $"Round #{roundId} has started! {roleService.GetTraitors().Count} traitors.")));
    SendTraitorMessage();
    SendDetectiveMessage();
    SendInnocentMessage();
  }

  private void SendInnocentMessage() {
    foreach (var player in roleService.GetInnocents())
      Server.NextFrame(()
        => player.PrintToChat(
          StringUtils.FormatTTT("You are now an innocent")));
  }

  private void SendTraitorMessage() {
    var traitors = roleService.GetTraitors();

    foreach (var traitor in traitors) {
      Server.NextFrame(()
        => traitor.PrintToChat(StringUtils.FormatTTT("You are a Traitor")));
      Server.NextFrame(()
        => traitor.PrintToChat(StringUtils.FormatTTT("Traitors:")));
      foreach (var player in traitors) {
        var message =
          StringUtils.FormatTTT(
            Role.Traitor.FormatStringFullAfter(player.PlayerName));
        Server.NextFrame(() => traitor.PrintToChat(message));
      }
    }
  }

  private void SendDetectiveMessage() {
    var detectives = roleService.GetDetectives();

    foreach (var detective in detectives) {
      Server.NextFrame(()
        => detective.PrintToChat(StringUtils.FormatTTT("You are a Detective")));
      Server.NextFrame(()
        => detective.PrintToChat(StringUtils.FormatTTT("Detective:")));
      foreach (var player in detectives) {
        var message = StringUtils.FormatTTT(
          Role.Detective.FormatStringFullAfter(" " + player.PlayerName));
        Server.NextFrame(() => detective.PrintToChat(message));
      }
    }
  }
}