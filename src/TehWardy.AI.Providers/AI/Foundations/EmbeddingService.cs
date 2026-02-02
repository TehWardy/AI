using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Foundations;

internal class EmbeddingService(IEmbeddingBroker embeddingBroker)
    : IEmbeddingService
{
    public ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest) =>
        embeddingBroker.EmbedAsync(embeddingRequest);
}