using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Brokers;

internal class RoutingBroker(IDataCacheProviderFactory dataCacheProviderFactory)
    : IRoutingBroker
{
    public async ValueTask<object> RetrieveAllRoutingInformationAsync<T>()
    {
        var provider = await dataCacheProviderFactory
            .CreateDataCacheProviderAsync<T>("Default");

        return await provider.GetAllItemsAsync();
    }
}