using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Brokers;

internal interface IMemoryBroker
{
    ValueTask<List<RagDocument>> SearchAsync(EmbeddedSearchRequest embeddedSearchRequest);
}