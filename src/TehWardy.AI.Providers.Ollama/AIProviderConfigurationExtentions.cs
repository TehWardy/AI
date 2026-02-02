using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Ollama.Brokers;
using TehWardy.AI.Providers.Ollama.Foundations;
using TehWardy.AI.Providers.Ollama.Orchestrations;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Ollama;

public static class AIProviderConfigurationExtentions
{
    public static void WithOllamaHostConfiguration(
        this AIProviderConfiguration aiProviderConfiguration,
        Action<OllamaHostConfiguration> hostConfigurationAction)
    {
        var config = new OllamaHostConfiguration();

        hostConfigurationAction(config);

        aiProviderConfiguration.ServiceCollection
            .AddSingleton(config);
    }

    public static void WithOllamaEmbeddingProvider(
        this AIProviderConfiguration aiProviderConfiguration,
        string providerName)
    {
        AddModelManagement(aiProviderConfiguration);

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IOllamaEmbeddingBroker, OllamaEmbeddingBroker>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IOllamaEmbeddingService, OllamaEmbeddingService>();

        aiProviderConfiguration.ServiceCollection.AddTransient<
            IOllamaEmbeddingOrchestrationService,
            OllamaEmbeddingOrchestrationService>();

        aiProviderConfiguration.ServiceCollection
            .AddKeyedTransient<IEmbeddingProvider, OllamaEmbeddingProvider>(providerName);
    }

    public static void WithOllamaLargeLanguageModelProvider(
        this AIProviderConfiguration aiProviderConfiguration,
        string providerName)
    {
        AddModelManagement(aiProviderConfiguration);

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IOllamaBroker, OllamaBroker>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IOllamaConversationService, OllamaConversationService>();

        aiProviderConfiguration.ServiceCollection.AddTransient<
            IOllamaLargeLanguageModelOrchestrationService,
            OllamaLargeLanguageModelOrchestrationService>();

        aiProviderConfiguration.ServiceCollection.AddKeyedTransient<
            ILargeLanguageModelProvider,
            OllamaLargeLanguageModelProvider>(providerName);
    }

    static void AddModelManagement(AIProviderConfiguration aiProviderConfiguration)
    {
        aiProviderConfiguration.ServiceCollection
            .AddTransient<IOllamaModelBroker, OllamaModelBroker>();

        aiProviderConfiguration.ServiceCollection
            .AddTransient<IOllamaModelService, OllamaModelService>();
    }
}