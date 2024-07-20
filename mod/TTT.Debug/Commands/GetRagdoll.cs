using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;

namespace TTT.Debug.Commands;

public class GetRagdoll : IPluginBehavior {
  [ConsoleCommand("css_getragdoll")]
  [CommandHelper(0, "", CommandUsage.CLIENT_ONLY)]
  public void Command_GetRagdoll(CCSPlayerController? executor,
    CommandInfo command) {
    if (executor == null) return;

    var body = executor.GetClientRagdollAimTarget();
    if (body == null) {
      command.ReplyToCommand("No body found");
      return;
    }

    if (!body.IsValid) {
      command.ReplyToCommand("Found body, but is invalid");
      return;
    }

    command.ReplyToCommand("Found " + body.DesignerName + " of "
      + body.PlayerName);
  }
}