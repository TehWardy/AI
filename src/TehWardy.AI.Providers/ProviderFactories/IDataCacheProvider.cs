namespace TehWardy.AI.Providers.ProviderFactories;

public interface IDataCacheProvider<T>
{
    public ValueTask<List<T>> GetAllItemsAsync();
    public ValueTask<T> GetItemByKeyAsync(string key);
    public ValueTask AddOrUpdateAsync(string key, T item);
    public ValueTask ClearAsync();
}