using Microsoft.Extensions.DependencyInjection;
using TTT.Debug.Commands;
using TTT.Public.Extensions;

namespace TTT.Debug;

public static class DebugServiceExtension {
  public static void AddDebugBehavior(this IServiceCollection collection) {
    collection.AddPluginBehavior<GetRagdoll>();
  }
}