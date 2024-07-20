using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;
using TTT.Public.Player;

namespace TTT.Manager;

public class TraitorRoom(IPlayerService service) : IPluginBehavior {
  public void Start(BasePlugin plugin) {
    plugin.AddTimer(0.1f, () => {
      foreach (var plr in Utilities.GetPlayers()
       .Where(player => player.IsReal()))
        OpenTraitorRoom(plr);
    });
  }

  public void OpenTraitorRoom(CCSPlayerController player) {
    if ((player.Buttons & PlayerButtons.Use) == 0) return;

    var traitorRoom = player.GetClientPlayerTraitorRoom();

    if (traitorRoom?.Globalname != "traitor_door") return;

    traitorRoom.AcceptInput("Open");
  }
}