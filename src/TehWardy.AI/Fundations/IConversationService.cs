using TehWardy.AI.Models;

namespace TehWardy.AI.Fundations;

internal interface IConversationService
{
    ValueTask<Conversation> CreateConversationAsync(Prompt prompt);
    ValueTask<List<Conversation>> RetrieveAllConversationsAsync();
    ValueTask<Conversation> RetrieveConversationAsync(Guid conversationId);
    ValueTask<Conversation> RetrieveConversationByIdAsync(Guid conversationId);
}