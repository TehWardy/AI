using TehWardy.AI.Providers.AI.Orchestrations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI;

internal class Conversation(
    IConversationOrchestrationService conversationOrchestrationService)
        : IConversation
{
    public IAsyncEnumerable<Token> InferAsync(
        InferrenceRequest inferrenceRequest)
    {
        return conversationOrchestrationService
            .InferStreamingAsync(inferrenceRequest);
    }
}