using TehWardy.AI.Providers.ProviderFactories;
using TehWardy.AI.Providers.System.Foundations;

namespace TehWardy.AI.Providers.System;

internal class DataCacheProvider<T>(IDataCacheService<T> dataCacheService)
    : IDataCacheProvider<T>
{
    public ValueTask<List<T>> GetAllItemsAsync() =>
        dataCacheService.GetAllItemsAsync();

    public ValueTask<T> GetItemByKeyAsync(string key) =>
        dataCacheService.GetItemByKeyAsync(key);

    public ValueTask AddOrUpdateAsync(string key, T item) =>
        dataCacheService.AddOrUpdateAsync(key, item);

    public ValueTask ClearAsync() =>
        dataCacheService.ClearAsync();
}