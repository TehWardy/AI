using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Brokers;

internal class LargeLanguageModelBroker(IServiceProvider serviceProvider)
    : ILargeLanguageModelBroker
{
    public async IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest)
    {
        ILargeLanguageModelProvider llmProvider = serviceProvider
            .GetRequiredKeyedService<ILargeLanguageModelProvider>(inferrenceRequest.LLMProviderName);

        await foreach (Token token in llmProvider.InferStreamingAsync(inferrenceRequest))
            yield return token;
    }

    public async ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest)
    {
        ILargeLanguageModelProvider llmProvider = serviceProvider
            .GetRequiredKeyedService<ILargeLanguageModelProvider>(inferrenceRequest.LLMProviderName);

        return await llmProvider.InferAsync(inferrenceRequest);
    }
}