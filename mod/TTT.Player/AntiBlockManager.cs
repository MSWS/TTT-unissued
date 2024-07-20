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
    if (!@event.Userid.IsValid) return HookResult.Continue;

    var player = @event.Userid;

    if (!player.IsReal()) return HookResult.Continue;

    if (!player.PlayerPawn.IsValid) return HookResult.Continue;

    var pawn = player.PlayerPawn;

    Server.NextFrame(() => PlayerSpawnNextFrame(player, pawn));

    return HookResult.Continue;
  }

  private void PlayerSpawnNextFrame(CCSPlayerController player,
    CHandle<CCSPlayerPawn> pawn) {
    pawn.Value.Collision.CollisionGroup =
      (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

    pawn.Value.Collision.CollisionAttribute.CollisionGroup =
      (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

    var collisionRulesChanged = new VirtualFunctionVoid<nint>(pawn.Value.Handle,
      OnCollisionRulesChangedOffset.Get());

    collisionRulesChanged.Invoke(pawn.Value.Handle);
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