using System.Collections.Concurrent;
using TehWardy.AI.Providers.System.Models;

namespace TehWardy.AI.Providers.System.Brokers;

internal class CachedMemoryProviderBroker : ICachedMemoryProviderBroker
{
    private readonly ConcurrentDictionary<string, List<StoredDocument>> indices = new();

    public ValueTask<List<StoredDocument>> GetDocumentsInIndexAsync(string indexName) =>
        ValueTask.FromResult(indices.GetOrAdd(indexName, _ => []));

    public ValueTask AddDocumentToIndexAsync(string indexName, StoredDocument document)
    {
        List<StoredDocument> docs = indices.GetOrAdd(indexName, _ => []);
        docs.Add(document);

        return ValueTask.CompletedTask;
    }

    public ValueTask ClearAsync(string indexName)
    {
        indices.GetOrAdd(indexName, _ => []).Clear();
        return ValueTask.CompletedTask;
    }
}