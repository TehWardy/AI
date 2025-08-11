using AIServer.Llama.Brokers;
using AIServer.Llama.Foundations;

namespace AIServer.Llama;

public static class IServiceCollectionExtentions
{
    public static void AddLlama(this IServiceCollection services, string modelPath)
    {
        services.AddSingleton(new LlamaConfiguration { ModelPath = modelPath });

        // Brokers
        services.AddTransient<ILlamaBroker, LlamaBroker>();

        // Foundations
        services.AddTransient<ILlamaService, LlamaService>();

        // Exposures
        services.AddTransient<ILlamaChatClient, LlamaChatClient>();
    }
}