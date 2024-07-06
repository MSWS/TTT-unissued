using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Public.Action;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Logs;

namespace TTT.Logs;

public class LogBehavior : ILogService, IPluginBehavior
{
    public void Start(BasePlugin plugin)
    {
        
    }
    
    private readonly Dictionary<int, IRoundLogs> _logs = new();
    
    public void AddLog(int round, IAction action)
    {
        _logs[round].AddLog(action);
    }

    public void PrintLogs(int round)
    {
        foreach (var player in Utilities.GetPlayers().Where(plr => plr.IsReal()))
        {
            PrintToPlayer(player, round);
        }
        
        PrintToConsole(round);
    }

    public void PrintToPlayer(CCSPlayerController player, int round)
    {
        player.PrintToConsole(GetLogs(round).FormattedLogs(round));
    }

    public void PrintToConsole(int round)
    {
        Server.PrintToConsole(GetLogs(round).FormattedLogs(round));
    }

    public IRoundLogs GetLogs(int round)
    {
        return _logs[round];
    }
}