using CounterStrikeSharp.API.Core;

namespace TTT.Public.Mod.Round;

public interface IRoundService {
  RoundStatus GetRoundStatus();
  void SetRoundStatus(RoundStatus roundStatus);
  void Start(BasePlugin plugin);

  void ForceEnd();
}

public enum RoundStatus {
  Waiting, Started, Paused, Ended
}