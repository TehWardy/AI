using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers;
using TehWardy.AI.Providers.AI.Foundations;
using TehWardy.AI.Providers.AI.Orchestrations;
using TehWardy.AI.Providers.Brokers;

namespace TehWardy.AI.Providers.AI;

public static class IServiceProviderExtentions
{
    public static void AddTehWardyAI(this IServiceCollection serviceCollection,
        Action<AIProviderConfiguration> aiProviderConfigurationAction)
    {
        // broker external deps
        serviceCollection.AddTehWardyAIProviders(aiProviderConfigurationAction);

        // this library deps
        AddBrokers(serviceCollection);
        AddFoundations(serviceCollection);
        AddOrchestrations(serviceCollection);
        AddExposures(serviceCollection);
    }

    static void AddBrokers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRoutingBroker, RoutingBroker>();
        serviceCollection.AddTransient<IEmbeddingBroker, EmbeddingBroker>();
        serviceCollection.AddTransient<ILargeLanguageModelBroker, LargeLanguageModelBroker>();
        serviceCollection.AddTransient<IMemoryBroker, MemoryBroker>();
    }

    static void AddFoundations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRoutingService, RoutingService>();
        serviceCollection.AddTransient<IEmbeddingService, EmbeddingService>();
        serviceCollection.AddTransient<ILargeLanguageModelService, LargeLanguageModelService>();
        serviceCollection.AddTransient<IMemoryService, MemoryService>();
    }

    static void AddOrchestrations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<
            IConversationRouterOrchestrationService,
            ConversationRouterOrchestrationService>();

        serviceCollection.AddTransient<
            IConversationOrchestrationService,
            ConversationOrchestrationService>();
    }

    static void AddExposures(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IConversationRouter, ConversationRouter>();
        serviceCollection.AddTransient<IConversation, Conversation>();
    }
}