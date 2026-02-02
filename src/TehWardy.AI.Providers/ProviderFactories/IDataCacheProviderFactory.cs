namespace TehWardy.AI.Providers.ProviderFactories;

public interface IDataCacheProviderFactory
{
    public ValueTask<IDataCacheProvider<T>> CreateDataCacheProviderAsync<T>(string providerName);
}