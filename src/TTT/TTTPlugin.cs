using System.Collections.Immutable;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.Public.Behaviors;

namespace TTT;

public class TTTPlugin : BasePlugin {
  private readonly IServiceProvider _provider;
  private IReadOnlyList<IPluginBehavior>? _extensions;
  private IServiceScope? _scope;

  /// <summary>
  ///   The TTT plugin.
  /// </summary>
  /// <param name="provider"></param>
  public TTTPlugin(IServiceProvider provider) { _provider = provider; }

  /// <inheritdoc />
  public override string ModuleName => "TTT";

  /// <inheritdoc />
  public override string ModuleVersion => "1.0.0";

  /// <inheritdoc />
  public override string ModuleAuthor => "NTM";

  /// <inheritdoc />
  public override void Load(bool hotReload) {
    Logger.LogInformation("[TTT] Loading...");

    _scope = _provider.CreateScope();
    _extensions = _scope.ServiceProvider.GetServices<IPluginBehavior>()
     .ToImmutableList();

    Logger.LogInformation("[TTT] Found {@BehaviorCount} behaviors.",
      _extensions.Count);

    foreach (var extension in _extensions) {
      //	Register all event handlers on the extension object
      RegisterAllAttributes(extension);

      //	Tell the extension to start it's magic
      extension.Start(this);

      Logger.LogInformation("[TTT] Loaded behavior {@Behavior}",
        extension.GetType().FullName);
    }

    base.Load(hotReload);
  }

  /// <inheritdoc />
  public override void Unload(bool hotReload) {
    Logger.LogInformation("[TTT] Shutting down...");

    if (_extensions != null)
      foreach (var extension in _extensions)
        extension.Dispose();

    //	Dispose of original extensions scope
    //	When loading again we will get a new scope to avoid leaking state.
    _scope?.Dispose();

    base.Unload(hotReload);
  }
}