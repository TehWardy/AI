using AIServer.Llama;
using AIServer.Ollama.Brokers;
using AIServer.Ollama.Configurations;
using AIServer.Ollama.Foundations;
using AIServer.Ollama.Orchestrations;
using AIServer.Ollama.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace AIServer.Ollama;

public static class IServiceCollectionExtensions
{
    public static void AddOllamaClient(this IServiceCollection services, string ollamaServerUrl)
    {
        services.AddTransient(ctx => new OllamaConversationCofiguration
        {
            OllamaHostUrl = ollamaServerUrl
        });

        // Client Brokers
        services.AddTransient<
            IOllamaConversationBroker,
            OllamaConversationBroker>();

        services.AddTransient<
            IMCPToolHttpCallRequestBroker,
            MCPToolHttpCallRequestBroker>();

        // Client Foundations
        services.AddTransient<
            IOllamaConversationService, 
            OllamaConversationService>();

        services.AddTransient<
            IMCPToolHttpCallRequestService, 
            MCPToolHttpCallRequestService>();

        // Host Processings
        services.AddTransient<
            IOllamaConversationProcessingService,
            OllamaConversationProcessingService>();

        services.AddTransient<
            IMCPToolCallRequestProcessingService,
            MCPToolCallRequestProcessingService>();

        // Client Orchestrations
        services.AddTransient<
            IOllamaConversationOrchestrationService,
            OllamaConversationOrchestrationService>();

        // Client Exposures
        services.AddTransient<IOllamaChatClient, OllamaChatClient>();
    }

    public static void AddOllamaHost(this IServiceCollection services, string ollamaExePath)
    {
        services.AddTransient(ctx => new OllamaHostConfiguration
        {
            OllamaExePath = ollamaExePath
        });

        // Host Brokers
        services.AddTransient<
            IOllamaModelHostBroker,
            OllamaModelHostBroker>();

        // Host Foundations
        services.AddTransient<
            IOllamaModelHostService, 
            OllamaModelHostService>();

        // Host Exposures
        services.AddTransient<
            IOllamaServiceHost, 
            OllamaServiceHost>();
    }
}