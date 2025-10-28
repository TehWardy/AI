
public interface IStorageBroker
{
    ValueTask<T> DeleteAsync<T>(T @object);
    ValueTask<T> InsertAsync<T>(T @object);
    IQueryable<T> SelectAll<T>() where T : class;
    ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class;
    ValueTask<T> UpdateAsync<T>(T @object);
}