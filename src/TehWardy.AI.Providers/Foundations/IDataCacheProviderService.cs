using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;
internal interface IDataCacheProviderService
{
    ValueTask<IDataCacheProvider<T>> GetDataCacheProviderAsync<T>(string providerName);
}