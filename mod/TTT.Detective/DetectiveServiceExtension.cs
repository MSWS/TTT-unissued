using Microsoft.Extensions.DependencyInjection;
using TTT.Public.Extensions;
using TTT.Public.Mod.Detective;

namespace TTT.Detective;

public static class DetectiveServiceExtension {
  public static void AddDetectiveBehavior(this IServiceCollection collection) {
    collection.AddPluginBehavior<IDetectiveService, DetectiveManager>();
  }
}