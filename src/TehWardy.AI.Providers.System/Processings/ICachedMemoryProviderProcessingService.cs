using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.InMemory.Processings;

internal interface ICachedMemoryProviderProcessingService
{
    ValueTask AddDocumentAsync(string indexName, dynamic metadata, RagDocument doc);
    ValueTask ClearAsync(string indexName);
    ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest);
}