using CounterStrikeSharp.API.Core;

namespace TTT.Public.Mod.Manager;

public interface IMuteService {
  void Mute(CCSPlayerController player);
  void UnMute(CCSPlayerController player);
  void UnMuteAll();
  void MuteAll();
}