using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal class LargeLanguageModelProviderService(IServiceProviderBroker serviceRroviderBroker)
    : ILargeLanguageModelProviderService
{
    public ValueTask<ILargeLanguageModelProvider> GetLargeLanguageModelProviderAsync(string providerName) =>
        serviceRroviderBroker.GetNamedServiceAsync<ILargeLanguageModelProvider>(providerName);
}