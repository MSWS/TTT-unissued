using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Formatting;
using TTT.Public.Mod.Role;
using TTT.Public.Mod.Round;
using TTT.Public.Player;

namespace TTT.Roles.Commands;

public class RolesCommand : IPluginBehavior
{
    private readonly IPlayerService _roleService;
    private readonly IRoundService _roundService;

    public RolesCommand(IPlayerService roleService, IRoundService roundService)
    {
        _roleService = roleService;
        _roundService = roundService;
    }
    
    public void Start(BasePlugin plugin)
    {
        
    }
    
    [ConsoleCommand("css_roles", "Get the roles of all players")]
    [CommandHelper(0, "", CommandUsage.CLIENT_ONLY)]
    public void Command_Toggle(CCSPlayerController? player, CommandInfo command)
    {
        if (!AdminManager.PlayerHasPermissions(player, "@css/kick")) return;

        if (_roundService.GetRoundStatus() != RoundStatus.Started)
        {
            command.ReplyToCommand(StringUtils.FormatTTT("The round has not started yet."));
            return;
        }
        
        StringBuilder sb = new();

        sb.AppendLine(StringUtils.FormatTTT("List of all players roles:"));
        
        foreach (var gamePlayer in _roleService.Players())
        {
            var plr = gamePlayer.Player();
            if (plr == null) continue;
            sb.AppendLine(StringUtils.FormatTTT(gamePlayer.PlayerRole().FormatStringAfter(plr.PlayerName)));
        }
        
        sb.AppendLine(StringUtils.FormatTTT("End."));
        
        command.ReplyToCommand(sb.ToString());
    }
}