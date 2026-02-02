using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Brokers;

namespace TehWardy.AI.Agents.Runbooks.Foundations;

internal class AccumulationTokenInferrenceService(
    IInferrenceBroker inferrenceBroker)
        : IAccumulationTokenInferrenceService
{
    public IAsyncEnumerable<Token> SendInferrenceRequestAsync(
        InferrenceRequest inferrenceRequest) =>
            inferrenceBroker.SendInferrenceRequestAsync(inferrenceRequest);
}