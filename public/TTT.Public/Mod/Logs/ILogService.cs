using CounterStrikeSharp.API.Core;
using TTT.Public.Action;

namespace TTT.Public.Mod.Logs;

public interface ILogService
{
    void AddLog(Action.Action action);
    bool PrintLogs(int round);
    bool PrintToPlayer(CCSPlayerController player, int round);
    bool PrintToConsole(int round);
    IRoundLogs GetLogs(int round);
    void CreateRound(int round);
}