using CounterStrikeSharp.API.Core;
using TTT.Public.Action;

namespace TTT.Public.Mod.Round;

public interface IRoundService
{
    RoundStatus GetRoundStatus();
    void SetRoundStatus(RoundStatus roundStatus);
    void Start(BasePlugin plugin);

    void TickWaiting();
    void ForceStart();
    void ForceEnd();
}

public enum RoundStatus
{
    Waiting,
    Started,
    Paused,
    Ended
}