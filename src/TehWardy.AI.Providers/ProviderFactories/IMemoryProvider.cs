using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.ProviderFactories;

public interface IMemoryProvider
{
    ValueTask AddDocumentAsync(string indexName, dynamic metadata, RagDocument doc);
    ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest);
    ValueTask ClearAsync(string indexName);
}