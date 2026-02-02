using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal class DataCacheProviderService(IServiceProviderBroker serviceProviderBroker)
    : IDataCacheProviderService
{
    public ValueTask<IDataCacheProvider<T>> GetDataCacheProviderAsync<T>(string providerName) =>
        serviceProviderBroker.GetNamedServiceAsync<IDataCacheProvider<T>>(providerName);
}