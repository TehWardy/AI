using TehWardy.AI.Providers.AI.Orchestrations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI;

internal class ConversationRouter(
    IConversationRouterOrchestrationService routingConversationOrchestrationService)
        : IConversationRouter

{
    public ValueTask<Token> InferRouteAsync<T>(
    InferrenceRequest inferrenceRequest)
    {
        return routingConversationOrchestrationService
            .InferRouteAsync<T>(inferrenceRequest);
    }
}