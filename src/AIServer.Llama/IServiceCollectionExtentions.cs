using AIServer.Llama.Brokers;
using AIServer.Llama.Configurations;
using AIServer.Llama.Foundations;
using AIServer.Llama.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace AIServer.Llama;

public static class IServiceCollectionExtentions
{
    public static void AddLlama(this IServiceCollection services, string modelsPath)
    {
        var config = new LlamaConfiguration
        {
            NativeLibraryPath = "runtimes\\win-x64\\native\\cuda12",
            ModelsPath = modelsPath
        };

        services.AddSingleton(config);

        // Brokers
        services.AddTransient<ILlamaBroker, LlamaBroker>();

        // Foundations
        services.AddTransient<ILlamaService, LlamaService>();

        // Processings
        services.AddTransient<ILlamaProcessingService, LlamaProcessingService>();

        // Exposures
        services.AddTransient<ILlamaChatClient, LlamaChatClient>();
    }
}