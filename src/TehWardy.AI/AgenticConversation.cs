using TehWardy.AI.Coordinations;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI;

internal class AgenticConversation(
    IAgenticConversationCoordinationService agenticConversationCoordinationService)
        : IAgenticConversation
{
    public ValueTask<Conversation> CreateConversationAsync(Prompt prompt) =>
        agenticConversationCoordinationService.CreateConversationAsync(prompt);

    public ValueTask<List<Conversation>> RetrieveAllConversationsAsync() =>
        agenticConversationCoordinationService.RetrieveAllConversationsAsync();

    public ValueTask<Conversation> RetrieveConversationByIdAsync(Guid converstionId) =>
        agenticConversationCoordinationService.RetrieveConversationByIdAsync(converstionId);

    public IAsyncEnumerable<Token> InferStreamingAsync(Prompt prompt)
    {
        return agenticConversationCoordinationService
            .InferStreamingAsync(prompt);
    }
}