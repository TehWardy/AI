using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers;

public class AIProviderConfiguration
{
    public readonly IServiceCollection ServiceCollection;

    internal AIProviderConfiguration(IServiceCollection serviceCollection) =>
        ServiceCollection = serviceCollection;

    public void AddEmbeddingModelProvider<T>(string name) where T : class, IEmbeddingProvider
    {
        ServiceCollection.AddKeyedTransient<IEmbeddingProvider, T>(name);
    }

    public void AddLargeLanguageModelProvider<T>(string name) where T : class, ILargeLanguageModelProvider
    {
        ServiceCollection.AddKeyedTransient<ILargeLanguageModelProvider, T>(name);
    }

    public void AddMemoryProvider<T>(string name) where T : class, IMemoryProvider
    {
        ServiceCollection.AddKeyedTransient<IMemoryProvider, T>(name);
    }
}