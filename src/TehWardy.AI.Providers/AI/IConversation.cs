using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI;

public interface IConversation
{
    IAsyncEnumerable<Token> InferAsync(InferrenceRequest inferrenceRequest);
}