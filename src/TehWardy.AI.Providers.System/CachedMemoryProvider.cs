using TehWardy.AI.Providers.InMemory.Processings;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.InMemory;

internal class CachedMemoryProvider(ICachedMemoryProviderProcessingService cachedMemoryProviderProcessingService)
    : IMemoryProvider
{
    public ValueTask AddDocumentAsync(string indexName, dynamic metadata, RagDocument document) =>
        cachedMemoryProviderProcessingService.AddDocumentAsync(indexName, metadata, document);

    public ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest) =>
        cachedMemoryProviderProcessingService.SearchAsync(embeddedSearchRequest);

    public ValueTask ClearAsync(string indexName) =>
        cachedMemoryProviderProcessingService.ClearAsync(indexName);
}