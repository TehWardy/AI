using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Runbooks.Processings;

internal interface IAccumulationTokenInferrenceProcessingService
{
    IAsyncEnumerable<Token> SendInferrenceRequestAsync(InferrenceRequest inferrenceRequest);
}