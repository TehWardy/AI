using AIServer.LlamaCpp.Brokers;
using AIServer.LlamaCpp.Configurations;
using AIServer.LlamaCpp.Foundations;
using AIServer.LlamaCpp.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace AIServer.LlamaCpp;

public static class IServiceCollectionExtentions
{
    public static void AddLlamaCpp(this IServiceCollection services, Action<LlamaCppConfiguration> configureAction = null)
    {
        

        var config = new LlamaCppConfiguration();

        configureAction(config);

        services.AddSingleton(config);

        // Brokers
        services.AddTransient<ILlamaCppHostBroker, LlamaCppHostBroker>();
        services.AddTransient<ILlamaCppBroker, LlamaCppBroker>();

        // Foundations
        services.AddTransient<ILlamaCppHostService, LlamaCppHostService>();
        services.AddTransient<ILlamaCppService, LlamaCppService>();

        // Processings
        services.AddTransient<ILlamaCppProcessingService, LlamaCppProcessingService>();

        // Exposures
        services.AddTransient<ILlamaCppChatClient, LlamaCppChatClient>();
    }
}