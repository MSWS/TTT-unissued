using TTT.Public.Action;

namespace TTT.Public.Mod.Round;

public interface IRoundService
{
    RoundStatus GetRoundStatus();
    void SetRoundStatus(RoundStatus roundStatus);

    void TickWaiting();
    void ForceStart();
    void ForceEnd();
    ILogsService GetLogsService();
}

public enum RoundStatus
{
    Waiting,
    Started,
    Paused,
    Ended
}