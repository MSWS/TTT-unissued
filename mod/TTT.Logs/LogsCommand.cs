using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TTT.Public.Behaviors;
using TTT.Public.Formatting;
using TTT.Public.Mod.Logs;

namespace TTT.Logs;

public class LogsCommand(ILogService service) : IPluginBehavior {
  private readonly ILogService _service = service;

  public void Start(BasePlugin plugin) { }

  [ConsoleCommand("css_logs", "Prints logs to console")]
  public void Command_Logs(CCSPlayerController? executor, CommandInfo info) {
    if (!AdminManager.PlayerHasPermissions(executor, "@css/kick")) {
      info.ReplyToCommand(
        StringUtils.FormatTTT(
          "You do not have permission to execute this command"));
      return;
    }

    var roundId = service.GetCurrentRound();

    if (info.ArgCount == 2) {
      if (!int.TryParse(info.GetArg(1), out roundId) || roundId <= 0) {
        info.ReplyToCommand(
          StringUtils.FormatTTT("Invalid round id, /logs <roundId>"));
        return;
      }
    }

    if (executor == null) {
      if (!service.PrintToConsole(roundId))
        info.ReplyToCommand(
          StringUtils.FormatTTT("No logs found for round " + roundId));
      return;
    }

    if (!AdminManager.PlayerHasPermissions(executor, "@css/kick")) {
      info.ReplyToCommand("You do not have permission to execute this command");
      return;
    }

    info.ReplyToCommand(!service.PrintToPlayer(executor, roundId) ?
      StringUtils.FormatTTT("No logs found for round " + roundId) :
      StringUtils.FormatTTT("Logs printed to console"));
  }
}