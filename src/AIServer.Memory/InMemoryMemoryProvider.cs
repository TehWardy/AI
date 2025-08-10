namespace AIServer.Memory;

public sealed class InMemoryMemoryProvider : IMemoryProvider
{
    private readonly List<string> documents = new();

    public InMemoryMemoryProvider(IEnumerable<string> initialDocs) =>
        documents.AddRange(initialDocs);

    public async ValueTask<string[]> GetRelevantDocumentsAsync(string query, int maxResults = 3)
    {
        var queryWords = query.Split(
            ' ', 
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return documents
            .Where(doc => queryWords.Any(w => doc.Contains(w, StringComparison.OrdinalIgnoreCase)))
            .Take(maxResults)
            .ToArray();
    }
}