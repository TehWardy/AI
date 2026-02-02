using TehWardy.AI.Providers.System.Models;

namespace TehWardy.AI.Providers.System.Brokers;

internal interface ICachedMemoryProviderBroker
{
    ValueTask AddDocumentToIndexAsync(string indexName, StoredDocument document);
    ValueTask ClearAsync(string indexName);
    ValueTask<List<StoredDocument>> GetDocumentsInIndexAsync(string indexName);
}