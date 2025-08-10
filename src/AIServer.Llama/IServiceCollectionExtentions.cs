using AIServer.Llama.Brokers;

namespace AIServer.Llama;

public static class IServiceCollectionExtentions
{
    public static void AddLlama(this IServiceCollection services)
    {
        AddBrokers(services);
    }

    private static void AddBrokers(IServiceCollection services)
    {
        services.AddTransient<ILlamaBroker, LlamaBroker>();
    }
}