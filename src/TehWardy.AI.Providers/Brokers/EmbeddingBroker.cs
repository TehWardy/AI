using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Brokers;

internal class EmbeddingBroker(IServiceProvider serviceProvider)
    : IEmbeddingBroker
{
    public async ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest)
    {
        IEmbeddingProvider embeddingProvider = serviceProvider
            .GetRequiredKeyedService<IEmbeddingProvider>(embeddingRequest.ProviderName);

        return await embeddingProvider.EmbedAsync(embeddingRequest);
    }
}