using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Foundations;

internal class MemoryService(IMemoryBroker memoryBroker)
    : IMemoryService
{
    public ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest) =>
        memoryBroker.SearchAsync(embeddedSearchRequest);
}