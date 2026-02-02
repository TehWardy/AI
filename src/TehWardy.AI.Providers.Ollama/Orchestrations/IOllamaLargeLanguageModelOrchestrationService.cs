using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Ollama.Orchestrations;

internal interface IOllamaLargeLanguageModelOrchestrationService
{
    IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest);
    ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest);
}