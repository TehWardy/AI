using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Foundations;

internal interface IMemoryService
{
    ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest);
}