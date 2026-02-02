using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal class MemoryProviderService(
    IServiceProviderBroker serviceProviderBroker)
        : IMemoryProviderService
{
    public ValueTask<IMemoryProvider> GetMemoryProviderAsync(string providerName) =>
        serviceProviderBroker.GetNamedServiceAsync<IMemoryProvider>(providerName);
}