using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.Public.Extensions;

public static class PlayerExtensions {
  public static CsTeam GetTeam(this CCSPlayerController controller) {
    return (CsTeam)controller.TeamNum;
  }

  public static string GetActiveWeaponName(this CCSPlayerController player) {
    return player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value
    ?.DesignerName ?? string.Empty;
  }

  public static CCSPlayerController? GetClientPlayerAimTarget(
    this CCSPlayerController player) {
    var GameRules = Utilities
     .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
     .FirstOrDefault()
    ?.GameRules;

    if (GameRules is null) return null;

    VirtualFunctionWithReturn<IntPtr, IntPtr, IntPtr> findPickerEntity =
      new(GameRules.Handle, 28);
    var target =
      new CBaseEntity(findPickerEntity.Invoke(GameRules.Handle, player.Handle));

    if (target.DesignerName is "player")
      return target.As<CCSPlayerPawn>().OriginalController.Value;

    return null;
  }

  public static CCSPlayerController? GetClientRagdollAimTarget(
    this CCSPlayerController player) {
    var GameRules = Utilities
     .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
     .FirstOrDefault()
    ?.GameRules;

    if (GameRules is null) return null;

    VirtualFunctionWithReturn<IntPtr, IntPtr, IntPtr> findPickerEntity =
      new(GameRules.Handle, 28);
    var target =
      new CBaseEntity(findPickerEntity.Invoke(GameRules.Handle, player.Handle));

    if (target.DesignerName is "player")
      return target.As<CCSObserverPawn>().OriginalController.Value;

    return null;
  }

  public static CBaseDoor? GetClientPlayerTraitorRoom(
    this CCSPlayerController player) {
    var GameRules = Utilities
     .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
     .FirstOrDefault()
    ?.GameRules;

    if (GameRules is null) return null;

    VirtualFunctionWithReturn<IntPtr, IntPtr, IntPtr> findPickerEntity =
      new(GameRules.Handle, 28);
    var target =
      new CBaseEntity(findPickerEntity.Invoke(GameRules.Handle, player.Handle));

    if (target.DesignerName is "func_door"
      || target.DesignerName is "prop_door_rotating")
      return target.As<CBaseDoor>();

    return null;
  }


  public static bool IsReal(this CCSPlayerController player) {
    //  Do nothing else before this:
    //  Verifies the handle points to an entity within the global entity list.
    if (!player.IsValid) return false;

    if (player.Connected != PlayerConnectedState.PlayerConnected) return false;

    if (player.IsBot || player.IsHLTV) return false;

    return true;
  }
}