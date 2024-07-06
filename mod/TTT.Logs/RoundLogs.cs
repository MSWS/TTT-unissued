using System.Text;
using TTT.Public.Action;
using TTT.Public.Mod.Logs;

namespace TTT.Logs;

public class RoundLogs : IRoundLogs
{
    private readonly List<IAction> _logs = new();
    
    public IList<IAction> GetLogs()
    {
        return _logs;
    }

    public void AddLog(IAction action)
    {
        _logs.Add(action);
    }

    public void RemoveLog(IAction action)
    {
        _logs.Remove(action);
    }
    
    public string FormattedLogs(int roundId)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"[TTT] Logs round {roundId}");

        foreach (var action in _logs) builder.AppendLine(action.ActionMessage());

        builder.AppendLine("[TTT] Logs ended!");

        return builder.ToString();
    }
}