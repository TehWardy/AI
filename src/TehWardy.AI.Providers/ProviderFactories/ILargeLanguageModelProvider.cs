using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.ProviderFactories;

public interface ILargeLanguageModelProvider
{
    IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest);
    ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest);
}