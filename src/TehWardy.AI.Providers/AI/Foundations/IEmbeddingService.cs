using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Foundations;

internal interface IEmbeddingService
{
    ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest);
}