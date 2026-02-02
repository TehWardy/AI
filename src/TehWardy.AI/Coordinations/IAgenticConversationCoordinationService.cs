using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Coordinations;

internal interface IAgenticConversationCoordinationService
{
    ValueTask<Conversation> CreateConversationAsync(Prompt prompt);
    ValueTask<List<Conversation>> RetrieveAllConversationsAsync();
    ValueTask<Conversation> RetrieveConversationByIdAsync(Guid converstionId);
    IAsyncEnumerable<Token> InferStreamingAsync(Prompt prompt);
}