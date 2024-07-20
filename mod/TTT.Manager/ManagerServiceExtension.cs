using Microsoft.Extensions.DependencyInjection;
using TTT.Public.Extensions;
using TTT.Public.Mod.Manager;

namespace TTT.Manager;

public static class ManagerServiceExtension {
  public static void AddManagerService(this IServiceCollection collection) {
    collection.AddPluginBehavior<IMuteService, MuteManager>();
    collection.AddPluginBehavior<InfoManager>();
    collection.AddPluginBehavior<TraitorRoom>();
    collection.AddPluginBehavior<ChatManager>();
  }
}