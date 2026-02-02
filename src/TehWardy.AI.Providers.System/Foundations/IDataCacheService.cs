namespace TehWardy.AI.Providers.System.Foundations;

internal interface IDataCacheService<T>
{
    ValueTask<List<T>> GetAllItemsAsync();
    ValueTask AddOrUpdateAsync(string key, T item);
    ValueTask ClearAsync();
    ValueTask<T> GetItemByKeyAsync(string key);
}