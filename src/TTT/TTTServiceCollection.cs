using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.Detective;
using TTT.Logs;
using TTT.Manager;
using TTT.Player;
using TTT.Roles;

namespace TTT;

public class TTTServiceCollection : IPluginServiceCollection<TTTPlugin> {
  public void ConfigureServices(IServiceCollection serviceCollection) {
    serviceCollection.AddPlayerService();
    serviceCollection.AddTTTRoles();
    serviceCollection.AddDetectiveBehavior();
    serviceCollection.AddLogsService();
    serviceCollection.AddManagerService();
  }
}