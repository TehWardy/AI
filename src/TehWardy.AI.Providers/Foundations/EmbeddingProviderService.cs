using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal class EmbeddingProviderService(IServiceProviderBroker serviceProviderBroker)
    : IEmbeddingProviderService
{
    public ValueTask<IEmbeddingProvider> GetEmbeddingModelProviderAsync(string providerName) =>
        serviceProviderBroker.GetNamedServiceAsync<IEmbeddingProvider>(providerName);
}