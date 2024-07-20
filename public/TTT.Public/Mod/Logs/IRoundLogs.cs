namespace TTT.Public.Mod.Logs;

public interface IRoundLogs {
  IList<Action.Action> GetLogs();
  void AddLog(Action.Action action);
  void RemoveLog(Action.Action action);
  string FormattedLogs();
}