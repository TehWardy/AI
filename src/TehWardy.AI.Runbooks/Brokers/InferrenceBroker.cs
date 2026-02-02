using TehWardy.AI.Providers.AI;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Brokers;

namespace TehWardy.AI.Agents.Runbooks.Brokers;

internal class InferrenceBroker(IConversation conversation)
    : IInferrenceBroker
{
    public IAsyncEnumerable<Token> SendInferrenceRequestAsync(
        InferrenceRequest inferrenceRequest) =>
            conversation.InferAsync(inferrenceRequest);
}