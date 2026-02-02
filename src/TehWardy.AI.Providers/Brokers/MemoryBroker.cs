using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Brokers;

internal class MemoryBroker(IServiceProvider serviceProvider) : IMemoryBroker
{
    public async ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest)
    {
        IMemoryProvider memoryProvider = serviceProvider
            .GetRequiredKeyedService<IMemoryProvider>(embeddedSearchRequest.ProviderName);

        return await memoryProvider.SearchAsync(embeddedSearchRequest);
    }
}