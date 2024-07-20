using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Behaviors;
using TTT.Public.Mod.Role;

namespace TTT.Manager;

public class ChatManager(IRoleService roleService) : IPluginBehavior {
  public void Start(BasePlugin plugin) {
    plugin.AddCommandListener("say_team", OnSayTeam);
  }

  private HookResult OnSayTeam(CCSPlayerController? caller, CommandInfo info) {
    if (caller == null || !caller.IsValid) return HookResult.Continue;
    var role = roleService.GetRole(caller);
    switch (role) {
      case Role.Innocent:
        return HookResult.Stop;
      case Role.Detective: {
        var message =
          $" {ChatColors.DarkBlue} DETECTIVE {caller.PlayerName} {info.GetArg(1)}";
        foreach (var player in roleService.GetDetectives())
          player.PrintToChat(message);
        break;
      }
      case Role.Traitor: {
        var message =
          $" {ChatColors.DarkRed} TRAITOR {caller.PlayerName} {info.GetArg(1)}";
        foreach (var player in roleService.GetTraitors())
          player.PrintToChat(message);
        break;
      }
      case Role.Unassigned:
        return HookResult.Continue;
      default:
        throw new ArgumentOutOfRangeException();
    }

    return HookResult.Handled;
  }
}