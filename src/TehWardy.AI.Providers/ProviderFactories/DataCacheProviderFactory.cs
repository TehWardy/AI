using TehWardy.AI.Providers.Foundations;

namespace TehWardy.AI.Providers.ProviderFactories;

internal class DataCacheProviderFactory(IDataCacheProviderService dataCacheProviderService)
    : IDataCacheProviderFactory
{
    public ValueTask<IDataCacheProvider<T>> CreateDataCacheProviderAsync<T>(string providerName) =>
        dataCacheProviderService.GetDataCacheProviderAsync<T>(providerName);
}