using TehWardy.AI.Providers.Brokers;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Foundations;

internal class LargeLanguageModelService(ILargeLanguageModelBroker largeLanguageModelBroker)
    : ILargeLanguageModelService
{
    public IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest) =>
        largeLanguageModelBroker.InferStreamingAsync(inferrenceRequest);

    public ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest) =>
        largeLanguageModelBroker.InferAsync(inferrenceRequest);
}