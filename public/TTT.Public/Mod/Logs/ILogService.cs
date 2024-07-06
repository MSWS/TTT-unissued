using CounterStrikeSharp.API.Core;
using TTT.Public.Action;

namespace TTT.Public.Mod.Logs;

public interface ILogService
{
    void AddLog(int round, IAction action);
    void PrintLogs(int round);
    void PrintToPlayer(CCSPlayerController player, int round);
    void PrintToConsole(int round);
    IRoundLogs GetLogs(int round);
}