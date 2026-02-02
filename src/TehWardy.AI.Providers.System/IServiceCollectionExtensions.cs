using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Foundations;
using TehWardy.AI.Providers.System.Brokers;
using TehWardy.AI.Providers.System.Foundations;
using TehWardy.AI.Providers.System.Processings;

namespace TehWardy.AI.Providers.System;

public static class IServiceCollectionExtensions
{
    public static void AddTehWardyAISystemProviders(this IServiceCollection serviceCollection)
    {
        AddBrokers(serviceCollection);
        AddFoundations(serviceCollection);
        AddProcessings(serviceCollection);
        AddExposures(serviceCollection);
    }

    static void AddBrokers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IProcessBroker, ProcessBroker>();
    }

    static void AddFoundations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IProcessService, ProcessService>();
    }

    static void AddProcessings(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IProcessProcessingService, ProcessProcessingService>();
    }

    static void AddExposures(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IExternalProcessProvider, ExternalProcessProvider>();
    }
}