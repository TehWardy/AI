using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Ollama.Foundations;

internal interface IOllamaConversationService
{
    IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest);
    ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest);
}