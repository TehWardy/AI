using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.System.Models;

namespace TehWardy.AI.Providers.InMemory.Foundations;
internal interface ICachedMemoryProviderService
{
    ValueTask AddDocumentAsync(string indexName, dynamic metadata, RagDocument doc);
    ValueTask ClearAsync(string indexName);
    ValueTask<List<StoredDocument>> GetDocumentsInIndexAsync(string indexName);
}