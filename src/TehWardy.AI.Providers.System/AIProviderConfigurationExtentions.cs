using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Foundations;
using TehWardy.AI.Providers.InMemory;
using TehWardy.AI.Providers.InMemory.Foundations;
using TehWardy.AI.Providers.InMemory.Processings;
using TehWardy.AI.Providers.ProviderFactories;
using TehWardy.AI.Providers.System.Brokers;
using TehWardy.AI.Providers.System.Foundations;
using TehWardy.AI.Providers.System.Processings;

namespace TehWardy.AI.Providers.System;

public static class AIProviderConfigurationExtentions
{
    public static void WithExternalProcessExecutionProvider(this AIProviderConfiguration aiProviderConfiguration)
    {
        aiProviderConfiguration.ServiceCollection
            .AddTransient<IExternalProcessProvider, ExternalProcessProvider>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IProcessProcessingService, ProcessProcessingService>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IProcessService, ProcessService>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IProcessBroker, ProcessBroker>();
    }

    public static void WithInMemoryMemoryProvider(this AIProviderConfiguration aiProviderConfiguration, string providerName)
    {
        aiProviderConfiguration.ServiceCollection
            .AddKeyedTransient<IMemoryProvider, CachedMemoryProvider>(providerName);

        aiProviderConfiguration.ServiceCollection
            .AddTransient<ICachedMemoryProviderBroker, CachedMemoryProviderBroker>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<ICachedMemoryProviderService, CachedMemoryProviderService>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<ICachedMemoryProviderProcessingService, CachedMemoryProviderProcessingService>();
    }

    public static void WithGenericDataCacheProvider<T>(
        this AIProviderConfiguration aiProviderConfiguration) where T : class
    {
        aiProviderConfiguration.ServiceCollection
            .AddKeyedSingleton<IDataCacheProvider<T>, DataCacheProvider<T>>("Default");

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IDataCacheBroker<T>, DataCacheBroker<T>>();

        aiProviderConfiguration.ServiceCollection
            .AddSingleton<IDataCacheService<T>, DataCacheService<T>>();
    }
}