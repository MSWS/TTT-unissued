using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TTT.Public.Behaviors;
using TTT.Public.Mod.Logs;

namespace TTT.Logs;

public class LogsCommand(ILogService service) : IPluginBehavior
{
    private readonly ILogService _service = service;

    public void Start(BasePlugin plugin)
    {
        
    }
    
    [ConsoleCommand("css_logs", "Prints logs to console")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void Command_Logs(CCSPlayerController? executor, CommandInfo info)
    {
        var roundIdString = info.GetArg(1);
        
        if (!int.TryParse(roundIdString, out var roundId))
        {
            info.ReplyToCommand("Invalid round id");
        }
        
        if (executor == null)
        {
            if (!service.PrintToConsole(roundId)) info.ReplyToCommand("No logs found for round " + roundId);
            return;
        }

        if (!AdminManager.PlayerHasPermissions(executor, "@css/kick"))
        {
            info.ReplyToCommand("You do not have permission to execute this command");
            return;
        }
        
        if (!service.PrintToPlayer(executor, roundId)) info.ReplyToCommand("No logs found for round " + roundId);

    }
}