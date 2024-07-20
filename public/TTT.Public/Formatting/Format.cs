using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.Public.Mod.Role;

namespace TTT.Public.Formatting;

public static class Format {
  public static string FormatStringAfter(this Role role, string message) {
    return FormatRole(role) + ChatColors.Lime + message;
  }

  public static string FormatStringBefore(this Role role, string message) {
    return " " + ChatColors.Green + message + " " + FormatRole(role)
      + ChatColors.Lime;
  }

  public static string FormatStringFullAfter(this Role role, string message) {
    return role.FormatRoleFull() + ChatColors.Lime + message;
  }

  public static string FormatStringFullBefore(this Role role, string message) {
    return " " + ChatColors.Green + message + " " + role.FormatRoleFull()
      + ChatColors.Green;
  }

  public static string FormatRole(this Role role) {
    return role switch {
      Role.Traitor   => ChatColors.DarkRed + "T ",
      Role.Detective => ChatColors.Blue + "D ",
      Role.Innocent  => ChatColors.Green + "I ",
      _              => ""
    };
  }

  public static string FormatRoleFull(this Role role) {
    return role switch {
      Role.Traitor    => ChatColors.DarkRed + "Traitor",
      Role.Detective  => ChatColors.Blue + "Detective",
      Role.Innocent   => ChatColors.Green + "Innocent",
      Role.Unassigned => "",
      _               => ""
    };
  }

  public static string GetCenterRole(this Role role) {
    return role switch {
      Role.Traitor =>
        $"<font color=\"#{Color.DarkRed.R:X2}{Color.DarkRed.G:X2}{Color.DarkRed.B:X2}\"><b>Traitor</b></font>",
      Role.Detective =>
        $"<font color=\"#{Color.Blue.R:X2}{Color.Blue.G:X2}{Color.Blue.B:X2}\"><b>Detective</b></font>",
      Role.Innocent =>
        $"<font color=\"#{Color.Lime.R:X2}{Color.Lime.G:X2}{Color.Lime.B:X2}\"><b>Innocent</b></font>",
      Role.Unassigned =>
        $"<font color=\"#{Color.DarkGray.R:X2}{Color.DarkGray.G:X2}{Color.DarkGray.B:X2}\"><b>Unknown</b></font>",
      _ => ""
    };
  }

  public static Color GetColor(this Role role) {
    return role switch {
      Role.Traitor    => Color.Red,
      Role.Detective  => Color.Blue,
      Role.Innocent   => Color.Lime,
      Role.Unassigned => Color.DarkGray,
      _               => Color.Black
    };
  }
}