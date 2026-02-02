using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Foundations;

internal interface ILargeLanguageModelService
{
    IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest);
    ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest);
}