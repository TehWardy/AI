using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.Foundations;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers;

public static class IServiceCollectionExtensions
{
    public static void AddTehWardyAIProviders(this IServiceCollection serviceCollection,
        Action<AIProviderConfiguration> aiProviderConfigurationAction)
    {
        AddBrokers(serviceCollection);
        AddFoundations(serviceCollection);
        AddExposures(serviceCollection);

        var config = new AIProviderConfiguration(serviceCollection);
        aiProviderConfigurationAction(config);
    }

    static void AddBrokers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
    }

    static void AddFoundations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDataCacheProviderService, DataCacheProviderService>();
        serviceCollection.AddTransient<IEmbeddingProviderService, EmbeddingProviderService>();
        serviceCollection.AddTransient<ILargeLanguageModelProviderService, LargeLanguageModelProviderService>();
        serviceCollection.AddTransient<IMemoryProviderService, MemoryProviderService>();
    }

    static void AddExposures(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDataCacheProviderFactory, DataCacheProviderFactory>();
        serviceCollection.AddTransient<IEmbeddingProviderFactory, EmbeddingProviderFactory>();
        serviceCollection.AddTransient<ILargeLanguageModelProviderFactory, LargeLanguageModelProviderFactory>();
        serviceCollection.AddTransient<IMemoryProviderFactory, MemoryProviderFactory>();
    }
}