using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI;

public interface IConversationRouter
{
    ValueTask<Token> InferRouteAsync<T>(InferrenceRequest inferrenceRequest);
}