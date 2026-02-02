using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Runbooks.Brokers;

internal interface IInferrenceBroker
{
    IAsyncEnumerable<Token> SendInferrenceRequestAsync(InferrenceRequest inferrenceRequest);
}