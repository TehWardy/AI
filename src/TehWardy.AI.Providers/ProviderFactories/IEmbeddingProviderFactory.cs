namespace TehWardy.AI.Providers.ProviderFactories;

public interface IEmbeddingProviderFactory
{
    ValueTask<IEmbeddingProvider> CreateEmbeddingModelProviderAsync(string providerName);
}