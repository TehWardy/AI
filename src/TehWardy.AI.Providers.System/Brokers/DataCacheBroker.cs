using System.Collections.Concurrent;

namespace TehWardy.AI.Providers.System.Brokers;

internal class DataCacheBroker<T>
    : IDataCacheBroker<T> where T : class
{
    private readonly ConcurrentDictionary<string, T> cache = new();

    public ValueTask<List<T>> GetAllItemsAsync() =>
        ValueTask.FromResult(cache.Values.ToList());

    public ValueTask<T> GetItemByKeyAsync(string key)
    {
        cache.TryGetValue(key, out T value);
        return ValueTask.FromResult(value);
    }

    public ValueTask AddOrUpdateAsync(string key, T item)
    {
        cache.AddOrUpdate(key, item, (_, _) => item);
        return ValueTask.CompletedTask;
    }

    public ValueTask ClearAsync()
    {
        cache.Clear();
        return ValueTask.CompletedTask;
    }
}