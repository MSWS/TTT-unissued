using System.Text;
using TTT.Public.Action;
using TTT.Public.Mod.Logs;
using Action = TTT.Public.Action.Action;

namespace TTT.Logs;

public class RoundLogs : IRoundLogs
{
    private readonly List<Action> _logs = new();
    
    public IList<Action> GetLogs()
    {
        return _logs;
    }

    public void AddLog(Action action)
    {
        _logs.Add(action);
    }

    public void RemoveLog(Action action)
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