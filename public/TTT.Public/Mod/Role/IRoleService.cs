using CounterStrikeSharp.API.Core;
using TTT.Public.Mod.Round;

namespace TTT.Public.Mod.Role;

public interface IRoleService {
  Role GetRole(CCSPlayerController player);
  void AddRoles();
  IRoundService GetRoundService();
  ISet<CCSPlayerController> GetTraitors();
  ISet<CCSPlayerController> GetDetectives();
  ISet<CCSPlayerController> GetInnocents();
  bool IsDetective(CCSPlayerController player);
  bool IsTraitor(CCSPlayerController player);
  void AddDetective(CCSPlayerController player);
  void AddTraitor(CCSPlayerController player);
  void AddInnocents(IEnumerable<CCSPlayerController> players);
  void Clear();
}

public enum Role {
  Traitor = 0, Detective = 1, Innocent = 2, Unassigned = 3
}