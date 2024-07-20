using Microsoft.Extensions.DependencyInjection;

namespace TTT.Formatting.Logistics;

public class LanguageConfig<TDialect> where TDialect : IDialect {
  private IServiceCollection _collection;

  public LanguageConfig(IServiceCollection collection) {
    _collection = collection;
  }
}