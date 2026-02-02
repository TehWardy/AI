using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Orchestrations;

internal interface IConversationOrchestrationService
{
    IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest);
}