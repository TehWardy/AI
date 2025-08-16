using AIServer.Llama.Brokers;
using AIServer.Llama.Foundations;
using Microsoft.Extensions.DependencyInjection;

namespace AIServer.Llama;

public static class IServiceCollectionExtentions
{
    public static void AddLlama(this IServiceCollection services, string modelsPath)
    {
        var config = new LlamaConfiguration
        {
            ModelsPath = modelsPath
        };

        services.AddSingleton(config);

        // Brokers
        services.AddTransient<ILlamaBroker, LlamaBroker>();

        // Foundations
        services.AddTransient<ILlamaService, LlamaService>();

        // Exposures
        services.AddTransient<ILlamaChatClient, LlamaChatClient>();
    }
}