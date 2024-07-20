using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Behaviors;
using TTT.Public.Extensions;

namespace TTT.Player;

public class AntiBlockManager : IPluginBehavior {
  private readonly WIN_LINUX<int> OnCollisionRulesChangedOffset = new(173, 172);

  public void Start(BasePlugin plugin) { }

  [GameEventHandler]
  private HookResult Event_PlayerSpawn(EventPlayerSpawn @event,
    GameEventInfo info) {
    var player = @event.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;
    if (!player.IsReal()) return HookResult.Continue;
    if (!player.PlayerPawn.IsValid) return HookResult.Continue;

    var pawn = player.PlayerPawn;
    if (pawn.Value == null) return HookResult.Continue;

    PlayerSpawnNextFrame(player, pawn.Value!);
    return HookResult.Continue;
  }

  private void PlayerSpawnNextFrame(CCSPlayerController player,
    CCSPlayerPawn pawn) {
    pawn.Collision.CollisionGroup =
      (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

    pawn.Collision.CollisionAttribute.CollisionGroup =
      (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

    var collisionRulesChanged = new VirtualFunctionVoid<nint>(pawn.Handle,
      OnCollisionRulesChangedOffset.Get());

    collisionRulesChanged.Invoke(pawn.Handle);
  }

  public class WIN_LINUX<T> {
    public WIN_LINUX(T windows, T linux) {
      Windows = windows;
      Linux   = linux;
    }

    [JsonPropertyName("Windows")]
    public T Windows { get; }

    [JsonPropertyName("Linux")]
    public T Linux { get; }

    public T Get() {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Windows;
      return Linux;
    }
  }
}