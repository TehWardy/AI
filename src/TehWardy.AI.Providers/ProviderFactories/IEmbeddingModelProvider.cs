using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.ProviderFactories;

public interface IEmbeddingProvider
{
    ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest);
}