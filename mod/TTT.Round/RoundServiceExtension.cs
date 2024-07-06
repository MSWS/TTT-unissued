using Microsoft.Extensions.DependencyInjection;
using TTT.Public.Extensions;
using TTT.Public.Mod.Round;

namespace TTT.Round;

public static class RoundServiceExtension
{
    public static void AddRoundService(this IServiceCollection collection)
    {
        collection.AddConfig<RoundConfig>("round");
        collection.AddPluginBehavior<IRoundService, RoundBehavior>();
    }
}