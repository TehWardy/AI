using TehWardy.AI.Providers.InMemory.Foundations;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.System.Brokers;
using TehWardy.AI.Providers.System.Models;

namespace TehWardy.AI.Providers.System.Foundations;

internal class CachedMemoryProviderService(ICachedMemoryProviderBroker cachedMemoryProviderBroker)
    : ICachedMemoryProviderService
{
    public ValueTask<List<StoredDocument>> GetDocumentsInIndexAsync(string indexName) =>
        cachedMemoryProviderBroker.GetDocumentsInIndexAsync(indexName);

    public async ValueTask AddDocumentAsync(string indexName, dynamic metadata, RagDocument doc)
    {
        ValidateIndexName(indexName);
        ValidateRagDocument(doc);

        var storedDoc = new StoredDocument
        {
            Metadata = metadata,
            Document = doc
        };

        await cachedMemoryProviderBroker.AddDocumentToIndexAsync(indexName, storedDoc);
    }

    public ValueTask ClearAsync(string indexName) =>
        cachedMemoryProviderBroker.ClearAsync(indexName);

    static void ValidateIndexName(string indexName)
    {
        if (string.IsNullOrWhiteSpace(indexName))
            throw new ArgumentException("IndexName is required.", nameof(indexName));
    }

    static void ValidateRagDocument(RagDocument doc)
    {
        if (doc is null)
            throw new ArgumentNullException(nameof(doc), "A docuemnt must be provided to add.");

        if (doc.ContentVector is null || doc.ContentVector.Length == 0)
            throw new ArgumentException("The provided document must have a ContentVector.", nameof(doc));
    }
}