using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Formatting.Core;
using TTT.Public.Extensions;

namespace TTT.Formatting.Objects;

public class PlayerFormatObject : FormatObject {
  private readonly string _name;

  public PlayerFormatObject(CCSPlayerController player) {
    _name = player.PlayerName;
  }

  public override string ToChat() { return $"{ChatColors.Yellow}{_name}"; }

  public override string ToPanorama() { return _name.Sanitize(); }

  public override string ToPlain() { return _name; }
}