using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.Ollama.Orchestrations;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Ollama;

internal class OllamaLargeLanguageModelProvider(
    IOllamaLargeLanguageModelOrchestrationService ollamaLargeLanguageModelOrchestrationService)
        : ILargeLanguageModelProvider
{
    public IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest) =>
        ollamaLargeLanguageModelOrchestrationService.InferStreamingAsync(inferrenceRequest);

    public ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest) =>
        ollamaLargeLanguageModelOrchestrationService.InferAsync(inferrenceRequest);
}