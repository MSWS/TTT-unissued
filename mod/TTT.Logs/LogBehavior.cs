using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Public.Action;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Mod.Logs;
using Action = TTT.Public.Action.Action;

namespace TTT.Logs;

public class LogBehavior : ILogService, IPluginBehavior
{
    private int _round = 1;
    
    public void Start(BasePlugin plugin)
    {
        plugin.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
    }
    
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        PrintLogs(_round);
        CreateRound(++_round);
        return HookResult.Continue;
    }
    
    private readonly Dictionary<int, IRoundLogs> _logs = new();
    
    public void AddLog(Action action)
    {
        _logs[_round].AddLog(action);
    }

    public bool PrintLogs(int round)
    {
        if (_logs.ContainsKey(round)) return false;
        foreach (var player in Utilities.GetPlayers().Where(plr => plr.IsReal()))
        {
            PrintToPlayer(player, round);
        }
        
        PrintToConsole(round);
        return true;
    }

    public bool PrintToPlayer(CCSPlayerController player, int round)
    {
        if (!_logs.ContainsKey(round)) return false;
        player.PrintToConsole(GetLogs(round).FormattedLogs(round));
        return true;
    }

    public bool PrintToConsole(int round)
    {
        if (!_logs.ContainsKey(round)) return false;
        Server.PrintToConsole(GetLogs(round).FormattedLogs(round));
        return true;
    }

    public IRoundLogs GetLogs(int round)
    {
        return _logs[round];
    }
    
    public void CreateRound(int round)
    {
        _logs.Add(round, new RoundLogs());
    }
}