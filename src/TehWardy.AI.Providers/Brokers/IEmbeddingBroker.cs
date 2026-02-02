using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Brokers;

internal interface IEmbeddingBroker
{
    ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest);
}