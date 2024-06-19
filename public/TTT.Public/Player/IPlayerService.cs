using CounterStrikeSharp.API.Core;
using TTT.Player;
using TTT.Public.Mod.Role;

namespace TTT.Public.Player;

public interface IPlayerService
{
    List<GamePlayer> Players();
    GamePlayer GetPlayer(CCSPlayerController player);
    void RemovePlayer(CCSPlayerController player);
    void Clr();
}