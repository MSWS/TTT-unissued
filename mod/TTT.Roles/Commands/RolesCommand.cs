using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TTT.Public.Behaviors;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Public.Player;

namespace TTT.Roles.Commands;

public class RolesCommand : IPluginBehavior {
  private readonly IPlayerService _roleService;
  private readonly IRoundService _roundService;

  public RolesCommand(IPlayerService roleService, IRoleService roundService) {
    _roleService  = roleService;
    _roundService = roundService.GetRoundService();
  }

  public void Start(BasePlugin plugin) { }

  [ConsoleCommand("css_roles", "Get the roles of all players")]
  [CommandHelper(0, "", CommandUsage.CLIENT_ONLY)]
  public void Command_Toggle(CCSPlayerController? player, CommandInfo command) {
    if (!AdminManager.PlayerHasPermissions(player, "@css/kick")) {
      command.ReplyToCommand(
        StringUtils.FormatTTT(
          "You do not have permission to execute this command"));
      return;
    }

    if (_roundService.GetRoundStatus() != RoundStatus.Started) {
      command.ReplyToCommand(
        StringUtils.FormatTTT("The round has not started yet."));
      return;
    }

    StringBuilder sb = new();

    command.ReplyToCommand(StringUtils.FormatTTT("List of all players roles:"));

    foreach (var gamePlayer in _roleService.Players()) {
      var plr = gamePlayer.Player();
      if (plr == null) continue;
      command.ReplyToCommand(StringUtils.FormatTTT(gamePlayer.PlayerRole()
       .FormatStringFullAfter(plr.PlayerName)));
    }

    command.ReplyToCommand(StringUtils.FormatTTT("End."));
  }
}