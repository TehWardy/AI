using TehWardy.AI.Providers.System.Brokers;

namespace TehWardy.AI.Providers.System.Foundations;

internal class DataCacheService<T>(IDataCacheBroker<T> dataCacheBroker)
    : IDataCacheService<T> where T : class
{
    public ValueTask<List<T>> GetAllItemsAsync() =>
        dataCacheBroker.GetAllItemsAsync();

    public ValueTask<T> GetItemByKeyAsync(string key) =>
        dataCacheBroker.GetItemByKeyAsync(key);

    public ValueTask AddOrUpdateAsync(string key, T item) =>
        dataCacheBroker.AddOrUpdateAsync(key, item);

    public ValueTask ClearAsync() =>
        dataCacheBroker.ClearAsync();
}