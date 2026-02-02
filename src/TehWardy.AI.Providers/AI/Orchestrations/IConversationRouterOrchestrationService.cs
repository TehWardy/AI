using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Orchestrations;

internal interface IConversationRouterOrchestrationService
{
    ValueTask<Token> InferRouteAsync<T>(InferrenceRequest inferrenceRequest);
}