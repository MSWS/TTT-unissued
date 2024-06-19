using CounterStrikeSharp.API.Core;

namespace TTT.Public.Mod.Scoreboard;

public interface IScoreboardService
{
    void RemoveStats(CCSPlayerController player);
    void RenamePlayer(CCSPlayerController player);
}