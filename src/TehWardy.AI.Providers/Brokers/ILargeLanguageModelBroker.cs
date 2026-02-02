using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Brokers;

internal interface ILargeLanguageModelBroker
{
    IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest);
    ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest);
}