using Microsoft.Extensions.DependencyInjection;
using TTT.Public.Extensions;
using TTT.Public.Player;
using TTT.Roles.Shop;

namespace TTT.Player;

public static class PlayerServiceExtension {
  public static void AddPlayerService(this IServiceCollection collection) {
    collection.AddPluginBehavior<IPlayerService, PlayerBehavior>();
    collection.AddPluginBehavior<KarmaManager>();
    collection.AddPluginBehavior<CreditManager>();
    collection.AddPluginBehavior<AntiBlockManager>();
  }
}