using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Agents.Runbooks.Foundations;

internal interface IAccumulationTokenInferrenceService
{
    IAsyncEnumerable<Token> SendInferrenceRequestAsync(InferrenceRequest inferrenceRequest);
}