using System.Text;
using TTT.Public.Mod.Logs;
using Action = TTT.Public.Action.Action;

namespace TTT.Logs;

public class RoundLog : IRoundLogs {
  private readonly List<Action> _logs = [];
  private readonly int _roundId;

  public RoundLog(int roundId) { _roundId = roundId; }

  public IList<Action> GetLogs() { return _logs; }

  public void AddLog(Action action) { _logs.Add(action); }

  public void RemoveLog(Action action) { _logs.Remove(action); }

  public string FormattedLogs() {
    var builder = new StringBuilder();
    builder.AppendLine($"[TTT] Logs round {_roundId}");

    foreach (var action in _logs) builder.AppendLine(action.ActionMessage());

    builder.AppendLine("[TTT] Logs ended!");

    return builder.ToString();
  }
}