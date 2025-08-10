namespace AIServer.Memory;

public interface IMemoryProvider
{
    ValueTask<string[]> GetRelevantDocumentsAsync(string query, int maxResults = 3);
}