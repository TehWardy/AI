using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal interface IEmbeddingProviderService
{
    ValueTask<IEmbeddingProvider> GetEmbeddingModelProviderAsync(string providerName);
}