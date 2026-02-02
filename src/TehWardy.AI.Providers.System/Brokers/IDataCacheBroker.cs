namespace TehWardy.AI.Providers.System.Brokers;

internal interface IDataCacheBroker<T> where T : class
{
    ValueTask<List<T>> GetAllItemsAsync();
    ValueTask AddOrUpdateAsync(string key, T item);
    ValueTask ClearAsync();
    ValueTask<T> GetItemByKeyAsync(string key);
}