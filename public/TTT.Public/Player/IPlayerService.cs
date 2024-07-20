using CounterStrikeSharp.API.Core;
using TTT.Player;

namespace TTT.Public.Player;

public interface IPlayerService {
  List<GamePlayer> Players();
  GamePlayer GetPlayer(CCSPlayerController player);
  void RemovePlayer(CCSPlayerController player);
  void CreatePlayer(CCSPlayerController player);
  void Clr();
  void AddKarma(CCSPlayerController player, int karma);
  void RemoveKarma(CCSPlayerController player, int karma);
}