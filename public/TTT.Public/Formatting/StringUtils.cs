using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.Public.Formatting;

public class StringUtils {
  private StringUtils() { throw new NotSupportedException("Utility class"); }

  public static string FormatTTT(string message) {
    return
      $" {ChatColors.Purple}[{ChatColors.Red}TTT{ChatColors.Purple}]{ChatColors.Green} {message}";
  }
}