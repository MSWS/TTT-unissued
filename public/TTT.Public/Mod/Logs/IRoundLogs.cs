using TTT.Public.Action;

namespace TTT.Public.Mod.Logs;

public interface IRoundLogs
{
    IList<IAction> GetLogs();
    void AddLog(IAction action);
    void RemoveLog(IAction action);
    string FormattedLogs(int roundId);
}