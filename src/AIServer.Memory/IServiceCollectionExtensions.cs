using Microsoft.Extensions.DependencyInjection;

namespace AIServer.Memory;
public static class IServiceCollectionExtensions
{
    public static void AddInMemoryMemoryProvider(this IServiceCollection services)
    {
        services.AddTransient<IMemoryProvider, InMemoryMemoryProvider>();
    }
}