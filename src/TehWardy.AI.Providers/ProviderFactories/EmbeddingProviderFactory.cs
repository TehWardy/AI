using TehWardy.AI.Providers.Foundations;

namespace TehWardy.AI.Providers.ProviderFactories;

internal class EmbeddingProviderFactory(IEmbeddingProviderService embeddingModelProviderService) : IEmbeddingProviderFactory
{
    public ValueTask<IEmbeddingProvider> CreateEmbeddingModelProviderAsync(string providerName) =>
        embeddingModelProviderService.GetEmbeddingModelProviderAsync(providerName);
}