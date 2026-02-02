using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Brokers;
using TehWardy.AI.Coordinations;
using TehWardy.AI.Fundations;
using TehWardy.AI.Orchestrations;
using TehWardy.AI.Providers;
using TehWardy.AI.Providers.AI;

namespace TehWardy.AI;

public static class IServiceProviderExtentions
{
    public static void AddTehWardyAgenticAI(this IServiceCollection serviceCollection,
        AgentConfiguration agentConfiguration,
        Action<AIProviderConfiguration> aiProviderConfigurationAction)
    {
        serviceCollection.AddSingleton(agentConfiguration);

        // broker external deps
        serviceCollection.AddTehWardyAI(aiProviderConfigurationAction);

        // this library deps
        AddBrokers(serviceCollection);
        AddFoundations(serviceCollection);
        AddOrchestrations(serviceCollection);
        AddCoordinations(serviceCollection);
        AddExposures(serviceCollection);
    }

    static void AddBrokers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAgentBroker, AgentBroker>();
        serviceCollection.AddTransient<IConversationBroker, ConversationBroker>();
        serviceCollection.AddTransient<IParameterParsingBroker, ParameterParsingBroker>();
        serviceCollection.AddTransient<IRunbookBroker, RunbookBroker>();
        serviceCollection.AddTransient<IRunbookRunnerBroker, RunbookRunnerBroker>();
    }

    static void AddFoundations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAgentService, AgentService>();
        serviceCollection.AddTransient<IConversationService, ConversationService>();
        serviceCollection.AddTransient<IRunbookService, RunbookService>();
        serviceCollection.AddTransient<IRunbookRunnerService, RunbookRunnerService>();

    }

    static void AddOrchestrations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRunbookOrchestrationService, RunbookOrchestrationService>();

        serviceCollection.AddTransient<
            IAgenticConversationContextOrchestrationService,
            AgenticConversationContextOrchestrationService>();
    }

    static void AddCoordinations(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<
            IAgenticConversationCoordinationService,
            AgenticConversationCoordinationService>();
    }

    static void AddExposures(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAgenticConversation, AgenticConversation>();
    }
}