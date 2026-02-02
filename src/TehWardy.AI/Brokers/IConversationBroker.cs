using TehWardy.AI.Models;

namespace TehWardy.AI.Brokers;

internal interface IConversationBroker
{
    ValueTask<List<Conversation>> RetrieveAllConversationsAsync();
    ValueTask<Conversation> GetConversationByIdAsync(Guid conversationId);
    ValueTask SaveConversationAsync(Conversation conversation);
}